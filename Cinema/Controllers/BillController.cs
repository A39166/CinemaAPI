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
    [SwaggerTag("Bill Controller")]
    public class BillController : BaseController
    {
        private readonly ILogger<BillController> _logger;
        private readonly DBContext _context;

        public BillController(DBContext context, ILogger<BillController> logger)
        {

            _context = context;
            _logger = logger;
        }
        
        [HttpPost("page_list_bill_admin")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<PageListBillAdminDTO>), description: "GetPageListCast Response")]
        public async Task<IActionResult> GetPageListCast(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<PageListBillAdminDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {

                var lstBill = _context.Bill.Include(p => p.ShowtimeUu).ThenInclude(m => m.MoviesUu)
                                             .Include(p => p.ShowtimeUu).ThenInclude(s => s.ScreenUu).ThenInclude(c => c.CinemaUu)
                                             .Where(x => x.Status == 1)
                                             .ToList();

                var totalcount = lstBill.Count();

                if (lstBill != null && lstBill.Count > 0)
                {
                    var result = lstBill.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<PageListBillAdminDTO>();
                    }
                    foreach (var bill in result)
                    {
                        var convertItemDTO = new PageListBillAdminDTO()
                        {
                            Uuid = bill.Uuid,
                            Code = bill.Code,
                            MovieName = bill.ShowtimeUu.MoviesUu.Title,
                            ScreenName = bill.ShowtimeUu.ScreenUu.ScreenName,
                            CinemaName = bill.ShowtimeUu.ScreenUu.CinemaUu.CinemaName,
                            State = bill.State,
                            PayPrice = bill.PayPrice,
                            ShowDate = bill.ShowtimeUu.ShowDate,
                            StartTime = bill.ShowtimeUu.StartTime,
                            EndTime = bill.ShowtimeUu.EndTime,
                            TimeCreated = bill.TimeCreated,
                            Status = bill.Status,
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
            catch (ErrorException ex)
            {
                response.error.SetErrorCode(ex.Code);
                return BadRequest(response);
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
            catch (ErrorException ex)
            {
                response.error.SetErrorCode(ex.Code);
                return BadRequest(response);
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

        /*[HttpPost("page_list_screen_for_client")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<PageListScreenForClientDTO>), description: "GetPageListScreenForClient Response")]
        public async Task<IActionResult> GetPageListScreenForClient(BaseKeywordRequest request)
        {
            var response = new BaseResponseMessageItem<PageListScreenForClientDTO>();
            try
            {
               
               response.Data = _context.Screen.Include(p => p.CinemaUu).Where(x => string.IsNullOrEmpty(request.Keyword)
                                                        || EF.Functions.Like(x.ScreenName + " ", $"%{request.Keyword}%"))
                                                              .Where(x => x.Status != 0)
                                .Select(scr => new PageListScreenForClientDTO
                                {
                                    Uuid = scr.Uuid,
                                    ScreenName = scr.ScreenName,
                                    Status = scr.Status
                                }).ToList();


                return Ok(response);
            }
            catch (ErrorException ex)
            {
                response.error.SetErrorCode(ex.Code);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }*/


    }
}
