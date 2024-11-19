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
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Director Controller")]
    public class ShowtimesController : BaseController
    {
        private readonly ILogger<ShowtimesController> _logger;
        private readonly DBContext _context;

        public ShowtimesController(DBContext context, ILogger<ShowtimesController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("upsert_showtime")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpsertShowtime Response")]
        public async Task<IActionResult> UpsertShowtime(UpsertShowtimeRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var movie = _context.Movies.Where(x => x.Uuid == request.MoviesUuid);
                var screen = _context.Screen.Where(x => x.Uuid == request.ScreenUuid);
                if (movie == null || screen == null)
                {
                    throw new ErrorException(ErrorCode.NOT_FOUND);
                }
                bool exists = _context.Showtimes.Any(s =>
                                                        s.MoviesUuid == request.MoviesUuid &&
                                                        s.ScreenUuid == request.ScreenUuid &&
                                                        s.StartTime == request.StartTime &&
                                                        s.ShowDate == request.ShowDate
                                                    );

                if (exists)
                {
                    throw new ErrorException(ErrorCode.DUPLICATED_SHOWTIME);
                }
                if (string.IsNullOrEmpty(request.Uuid))
                {
                    
                    var st = new Showtimes()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        MoviesUuid = request.MoviesUuid,
                        ScreenUuid = request.ScreenUuid,
                        LanguageType = request.LanguageType,
                        State = 0,
                        ShowDate = request.ShowDate,
                        StartTime = request.StartTime,
                        EndTime = request.EndTime,
                        Status = 1,
                    };
                    _context.Showtimes.Add(st);
                    _context.SaveChanges();
                }
                else
                //cập nhập dữ liệu
                {
                    var showtime = _context.Showtimes.Where(x => x.Uuid == request.Uuid).FirstOrDefault();
                    if (showtime != null)
                    {
                        showtime.MoviesUuid = request.MoviesUuid;
                        showtime.ScreenUuid = request.ScreenUuid;
                        showtime.LanguageType = request.LanguageType;
                        showtime.ShowDate = request.ShowDate;
                        showtime.StartTime = request.StartTime;
                        showtime.EndTime = request.EndTime;

                        _context.Showtimes.Update(showtime);
                        _context.SaveChanges();
                    }
                    else
                    {
                        response.error.SetErrorCode(ErrorCode.NOT_FOUND);
                    }
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

        [HttpPost("page_list_showtime")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<PageListShowtimesDTO>), description: "GetPageListShowtimes Response")]
        public async Task<IActionResult> GetPageListShowtimes(PageListShowtimesRequest request)
        {
            var response = new BaseResponseMessagePage<PageListShowtimesDTO>();
            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var lstShowtime = _context.Cinemas.Include(c => c.Screen).ThenInclude(s => s.Showtimes)
                                                  .Where(x => string.IsNullOrEmpty(request.CinemaUuid) || x.Uuid == request.CinemaUuid)
                                                  .Where(x => string.IsNullOrEmpty(request.ScreenUuid) || x.Screen.Any(s => s.Uuid == request.ScreenUuid))
                                                  .Where(x => !request.FindDate.HasValue || x.Screen.Any(s => s.Showtimes.Any(st => st.ShowDate == request.FindDate)))
                                                  .Where(x => x.Screen.Any(s => s.Showtimes.Any(st => st.Status == 1)))
                                                  .ToList();
                var totalcount = lstShowtime.Count();

                if (lstShowtime != null && lstShowtime.Count > 0)
                {
                    var result = lstShowtime.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<PageListShowtimesDTO>();
                    }
                    foreach (var cinema in result)
                    {
                        var convertItemDTO = new PageListShowtimesDTO
                        {
                            CinemaName = cinema.CinemaName,
                            Screens = cinema.Screen
                                .Where(screen => string.IsNullOrEmpty(request.ScreenUuid) || screen.Uuid == request.ScreenUuid)
                                .Select(screen => new FormSCreenDTO
                                {
                                    ScreenName = screen.ScreenName,
                                    Showtimes = screen.Showtimes
                                        .Where(showtime =>
                                            (!request.FindDate.HasValue || showtime.ShowDate == request.FindDate) &&
                                            (showtime.Status == 1))
                                        .Select(showtime => new FormShowtimesDTO
                                        {
                                            Uuid = showtime.Uuid,
                                            MoviesName = _context.Movies
                                                .Where(m => m.Uuid == showtime.MoviesUuid)
                                                .Select(m => m.Title)
                                                .FirstOrDefault(),
                                            ScreenType = _context.ScreenType
                                                .Where(x => x.Uuid == screen.ScreenTypeUuid)
                                                .Select(t => t.Type)
                                                .FirstOrDefault(),
                                            LanguageType = showtime.LanguageType,
                                            ShowDate = showtime.ShowDate,
                                            StartTime = showtime.StartTime,
                                            EndTime = showtime.EndTime,
                                            State = showtime.State,
                                            Status = showtime.Status
                                        })
                                        .ToList()
                                })
                                .Where(screen => screen.Showtimes.Any()) // Chỉ lấy các screen có lịch chiếu phù hợp
                                .ToList()
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
        [HttpPost("showtime_detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(ShowtimeDTO), description: "GetShowtimeDetail Response")]
        public async Task<IActionResult> GetShowtimeDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<ShowtimeDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                //TODO: Write code late

                var st = _context.Showtimes.Include(x => x.ScreenUu)
                    .Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                if (st != null)
                {
                    var scr = _context.Screen.Where(x => x.Uuid == st.ScreenUuid).Select(up => up.ScreenTypeUuid).FirstOrDefault();
                    var scrtype = _context.ScreenType.Where(x => x.Uuid == scr).Select(up => up.Type).FirstOrDefault();
                    response.Data = new ShowtimeDTO()
                    {
                        Uuid = st.Uuid,
                        MoviesUuid = st.MoviesUuid,
                        ScreenUuid = st.ScreenUuid,
                        CinemaUuid = _context.Cinemas.Where(x => x.Uuid == st.ScreenUu.CinemaUuid).Select(up => up.Uuid).FirstOrDefault(),
                        ScreenType = scrtype,
                        LanguageType = st.LanguageType,
                        ShowDate = st.ShowDate,
                        StartTime = st.StartTime,
                        EndTime = st.EndTime,
                        Status = st.Status,
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
        [HttpPost("update_showtime_status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateShowtimeStatus Response")]
        public async Task<IActionResult> UpdateShowtimeStatus(UpdateStatusRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var showtime = _context.Showtimes.Where(x => x.Uuid == request.Uuid).SingleOrDefault();

                if (showtime != null)
                {
                    showtime.Status = request.Status;
                    
                    _context.SaveChanges();
                }
                else
                {
                    response.error.SetErrorCode(ErrorCode.NOT_FOUND);
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
