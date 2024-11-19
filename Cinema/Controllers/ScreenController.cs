using CinemaAPI.Models.Request;
using CinemaAPI.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using CinemaAPI.Enums;
using CinemaAPI.Databases.CinemaDB;
using Microsoft.EntityFrameworkCore;
using CinemaAPI.Utils;
using CinemaAPI.Models.BaseRequest;
using Microsoft.AspNetCore.Identity.Data;
using CinemaAPI.Models.DataInfo;
using CinemaAPI.Extensions;
using System.IO;
using System.Security.Principal;
using static System.Net.Mime.MediaTypeNames;
using CinemaAPI.Configuaration;

namespace CinemaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Screen Controller")]
    public class ScreenController : BaseController
    {
        private readonly ILogger<ScreenController> _logger;
        private readonly DBContext _context;

        public ScreenController(DBContext context, ILogger<ScreenController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("create_screen")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "CreateScreen Response")]
        public async Task<IActionResult> CreateScreen(CreateScreenRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var oldscreen = _context.Screen.Where(x => x.CinemaUuid == request.CinemaUuid 
                                            && x.ScreenName == request.ScreenName && x.Status == 1).FirstOrDefault();
                if(oldscreen != null)
                {
                    throw new ErrorException(ErrorCode.DUPLICATE_SCREEN);
                }
                var screen = new Screen()
                {
                    Uuid = Guid.NewGuid().ToString(),
                    ScreenName = request.ScreenName,
                    CinemaUuid = request.CinemaUuid,
                    ScreenTypeUuid = _context.ScreenType.Where(x => x.Type == request.ScreenType).Select(up => up.Uuid).FirstOrDefault(),
                    Capacity = request.Capacity,
                    Row = request.Rows,
                    Collumn = request.Columns,
                    TimeCreated = DateTime.Now,
                    Status = 1,

                };
                _context.Screen.Add(screen);
                var seats = request.Seats.Select(seat => new Seat
                {
                    Uuid = Guid.NewGuid().ToString(),
                    ScreenUuid = screen.Uuid,
                    SeatCode = seat.SeatCode,
                    SeatTypeUuid = _context.SeatType.Where(t => t.Type == seat.SeatType).Select(up => up.Uuid).FirstOrDefault(),
                    TimeCreated = DateTime.Now,
                    Status = 1
                }).ToList();
                _context.Seat.AddRange(seats);
                _context.SaveChanges();

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }

        [HttpPost("update_screen")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateScreen Response")]
        public async Task<IActionResult> UpdateScreen(UpdateScreenRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var screen = _context.Screen.FirstOrDefault(x => x.Uuid == request.Uuid && x.Status == 1);
                if (screen == null)
                {
                    throw new ErrorException(ErrorCode.SCREEN_NOT_FOUND);
                }
                screen.ScreenName = request.ScreenName;
                screen.ScreenTypeUuid = _context.ScreenType.Where(x => x.Type == request.ScreenType)
                                                            .Select(up => up.Uuid)
                                                            .FirstOrDefault();
                screen.Capacity = request.Capacity;
                screen.Row = request.Rows;
                screen.Collumn = request.Columns;
                var seatUuidsInRequest = request.Seats.Select(seat => seat.SeatUuid).ToList();
                var existingSeats = _context.Seat.Where(s => s.ScreenUuid == screen.Uuid).ToList();
                
                var seatsToRemove = existingSeats.Where(s => !seatUuidsInRequest.Contains(s.Uuid)).ToList();
                _context.Seat.RemoveRange(seatsToRemove);
                foreach (var seat in request.Seats)
                {
                    var existingSeat = _context.Seat.FirstOrDefault(s => s.Uuid == seat.SeatUuid && s.ScreenUuid == screen.Uuid);
                    if (existingSeat != null)
                    {
                        existingSeat.SeatCode = seat.SeatCode;
                        existingSeat.SeatTypeUuid = _context.SeatType.Where(t => t.Type == seat.SeatType).Select(up => up.Uuid).FirstOrDefault();
                        _context.Seat.Update(existingSeat);
                    }
                    else if(string.IsNullOrEmpty(seat.SeatUuid))
                    {
                        var newSeat = new Seat
                        {
                            Uuid = Guid.NewGuid().ToString(),
                            ScreenUuid = screen.Uuid,
                            SeatCode = seat.SeatCode,
                            SeatTypeUuid = _context.SeatType.Where(t => t.Type == seat.SeatType)
                                                            .Select(up => up.Uuid).FirstOrDefault(),
                            TimeCreated = DateTime.Now,
                            Status = 1
                        };
                        _context.Seat.Add(newSeat);
                    }
                    else
                    {

                    }
                }
                _context.Screen.Update(screen);
                _context.SaveChanges();
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }

        [HttpPost("page_list_screen")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<PageListScreenDTO>), description: "GetPageListCast Response")]
        public async Task<IActionResult> GetPageListCast(PageListScreenRequest request)
        {
            var response = new BaseResponseMessagePage<PageListScreenDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var query = _context.Screen;
                var lstScreen = query.Include(p => p.CinemaUu).Where(x => string.IsNullOrEmpty(request.CinemaUuid) || x.CinemaUu.Uuid == request.CinemaUuid)
                                                              .Where(x => request.Status == null || x.Status == request.Status)
                                                              .ToList();

                var totalcount = lstScreen.Count();

                if (lstScreen != null && lstScreen.Count > 0)
                {
                    var result = lstScreen.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<PageListScreenDTO>();
                    }
                    foreach (var screen in result)
                    {
                        var convertItemDTO = new PageListScreenDTO()
                        {
                            Uuid = screen.Uuid,
                            ScreenName = screen.ScreenName,
                            Capacity = screen.Capacity,
                            ScreenType = _context.ScreenType.Where(x => x.Uuid == screen.ScreenTypeUuid).Select(t => t.Type).FirstOrDefault(),
                            TimeCreated = screen.TimeCreated,
                            Status = screen.Status,
                        };
                        response.Data.Items.Add(convertItemDTO);
                    }
                    // trả về thông tin page
                    response.Data.Pagination = new Paginations()
                    {
                        TotalPage = result.TotalPages,
                        TotalCount = result.TotalCount,
                    };
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }
        [HttpPost("screen_detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(ScreenDTO), description: "GetScreenDetail Response")]
        public async Task<IActionResult> GetScreenDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<ScreenDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                //TODO: Write code late

                var screendetail = _context.Screen.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                if (screendetail != null)
                {
                    response.Data = new ScreenDTO()
                    {
                        Uuid = screendetail.Uuid,
                        CinemaUuid = screendetail.CinemaUuid,
                        ScreenName = screendetail.ScreenName,
                        ScreenType = _context.ScreenType.Where(x => x.Uuid == screendetail.ScreenTypeUuid).Select(p => p.Type).FirstOrDefault(),
                        Capacity = screendetail.Capacity,
                        Row = screendetail.Row,
                        Collumn = screendetail.Collumn,
                        TimeCreated = screendetail.TimeCreated,
                        Status = screendetail.Status,
                    };
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);
                return BadRequest(response);
            }
        }
        [HttpPost("update_screen_status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateScreenStatus Response")]
        public async Task<IActionResult> UpdateCastStatus(UpdateStatusRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var screen = _context.Screen.Where(x => x.Uuid == request.Uuid).SingleOrDefault();

                if (screen == null)
                {
                    throw new ErrorException(ErrorCode.SCREEN_NOT_FOUND);
                }
                else
                {
                    screen.Status = request.Status;
                    var existingSeats = _context.Seat.Where(x => x.ScreenUuid == request.Uuid).ToList();
                    foreach(var seat in existingSeats)
                    {
                        seat.Status = request.Status;
                        _context.Seat.Update(seat);
                    }
                    _context.Screen.Update(screen);
                    _context.SaveChanges();

                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }

        [HttpPost("get_list_seat")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<ShortSeatDTO>), description: "GetListSeat Response")]
        public async Task<IActionResult> GetListSeat(UuidRequest request)
        {
            var response = new BaseResponseMessageItem<ShortSeatDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                response.Data = _context.Seat.Where(x => x.ScreenUuid == request.Uuid && x.Status == 1)
               .Select(seat => new ShortSeatDTO
                {
                    Uuid = seat.Uuid,
                    SeatCode = seat.SeatCode,
                    SeatType = _context.SeatType
                    .Where(t => t.Uuid == seat.SeatTypeUuid)
                    .Select(t => t.Type)
                    .FirstOrDefault()
                }).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }

        [HttpPost("category-screen-type")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<CategoryDTO>), description: "GetCategoryScreenType Response")]
        public async Task<IActionResult> GetCategoryScreenType(BaseCategoryRequest request)
        {
            var response = new BaseResponseMessageItem<CategoryDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var screentype = _context.ScreenType.Where(x => string.IsNullOrEmpty(request.Keyword)
                                                        || EF.Functions.Like(x.Type + " " + x.Name, $"%{request.Keyword}%"))
                                                 .ToList();
               if(screentype != null)
                {
                    response.Data = screentype.Select(st => new CategoryDTO
                    {
                        Uuid = st.Uuid, 
                        Name = st.Name,
                        Type = st.Type,
                    }).ToList();
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }

        

        [HttpPost("category-seat-type")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<CategoryDTO>), description: "GetCategorySeatType Response")]
        public async Task<IActionResult> GetCategorySeatType(BaseCategoryRequest request)
        {
            var response = new BaseResponseMessageItem<CategoryDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var seattype = _context.SeatType.Where(x => string.IsNullOrEmpty(request.Keyword)
                                                        || EF.Functions.Like(x.Type + " " + x.Name, $"%{request.Keyword}%"))
                                                 .ToList();
                if (seattype != null)
                {
                    response.Data = seattype.Select(st => new CategoryDTO
                    {
                        Uuid = st.Uuid,
                        Name = st.Name,
                        Type = st.Type,
                    }).ToList();
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }



    }
}
