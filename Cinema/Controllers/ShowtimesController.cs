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
                
                if (string.IsNullOrEmpty(request.Uuid))
                {

                    var showtime = _context.Showtimes.Where(s =>
                                                         s.MoviesUuid == request.MoviesUuid &&
                                                         s.ScreenUuid == request.ScreenUuid &&
                                                         s.ShowDate == request.ShowDate &&
                                                         s.State == 0 &&
                                                         s.Status == 1 && (
                                                         (
                                                         (request.StartTime >= s.StartTime && request.StartTime < s.EndTime) ||
                                                         (request.EndTime > s.StartTime && request.EndTime <= s.EndTime) ||
                                                         (request.StartTime <= s.StartTime && request.EndTime >= s.EndTime && s.StartTime <s.EndTime) ||
                                                         (request.StartTime < s.EndTime.AddHours(24) && request.StartTime >= s.StartTime) ||
                                                         (request.EndTime > s.StartTime && request.EndTime <= s.EndTime.AddHours(24))
                                                         ))).FirstOrDefault();
                    
                    if (showtime != null)
                    {
                        throw new ErrorException(ErrorCode.DUPLICATED_SHOWTIME);
                    }
                    var duration = _context.Movies.Where(x => x.Uuid == request.MoviesUuid).Select(m => m.Duration).FirstOrDefault();
                    var movieDuration = TimeSpan.FromMinutes(duration);

                    // Kiểm tra điều kiện
                    if ((request.EndTime - request.StartTime) < movieDuration)
                    {
                        throw new ErrorException(ErrorCode.INVALID_TIME);
                    }
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
                    if(showtime.State != 0)
                    {
                        throw new ErrorException(ErrorCode.CANT_UPDATE_SHOWTIME);
                    }
                    bool exists = _context.Showtimes.Any(s =>
                                    s.Uuid != request.Uuid && // Loại trừ suất chiếu đang cập nhật
                                    s.MoviesUuid == request.MoviesUuid &&
                                    s.ScreenUuid == request.ScreenUuid &&
                                    s.ShowDate == request.ShowDate &&
                                    s.Status == 1 &&
                                    (
                                        (request.StartTime >= s.StartTime && request.StartTime < s.EndTime) || // Trùng thời gian bắt đầu
                                        (request.EndTime > s.StartTime && request.EndTime <= s.EndTime) ||    // Trùng thời gian kết thúc
                                        (request.StartTime <= s.StartTime && request.EndTime >= s.EndTime)  ||  // Bao phủ toàn bộ suất chiếu khác
                                        (request.StartTime <= s.StartTime && request.EndTime >= s.EndTime && s.StartTime < s.EndTime) ||
                                        (request.StartTime < s.EndTime.AddHours(24) && request.StartTime >= s.StartTime) ||
                                        (request.EndTime > s.StartTime && request.EndTime <= s.EndTime.AddHours(24))
        )
    );

                    if (exists)
                    {
                        throw new ErrorException(ErrorCode.DUPLICATED_SHOWTIME);
                    }
                    var duration = _context.Movies.Where(x => x.Uuid == request.MoviesUuid).Select(m => m.Duration).FirstOrDefault();
                    var movieDuration = TimeSpan.FromMinutes(duration);

                    // Kiểm tra điều kiện
                    if ((request.EndTime - request.StartTime) < movieDuration)
                    {
                        throw new ErrorException(ErrorCode.INVALID_TIME);
                    }
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
                        State = st.State,
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
                var showtime = _context.Showtimes.Where(x => x.Uuid == request.Uuid && x.Status != 0).SingleOrDefault();
                if(showtime.State != 0)
                {
                    throw new ErrorException(ErrorCode.BAD_REQUEST);
                }
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
        [HttpPost("update_showtime_state")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "Update Showtime State Response")]
        public async Task<IActionResult> UpdateShowtimeState()
        {
            var response = new BaseResponse();
            try
            {
                var currentTime = DateTime.Now;

                // Lấy danh sách suất chiếu cần cập nhật
                var lstShowtimes = _context.Showtimes
                                           .Where(st => st.Status != 2) // Chỉ cập nhật các suất chưa kết thúc
                                           .ToList();

                if (lstShowtimes != null && lstShowtimes.Count > 0)
                {
                    foreach (var showtime in lstShowtimes)
                    {
                        if (showtime.StartTime <= TimeOnly.FromDateTime(currentTime) && TimeOnly.FromDateTime(currentTime) <= showtime.EndTime)
                        {
                            showtime.Status = 1; // Đang diễn ra
                        }
                        else if (TimeOnly.FromDateTime(currentTime) > showtime.EndTime)
                        {
                            showtime.Status = 2; // Đã kết thúc
                        }
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _context.SaveChangesAsync();

                    response.error.SetErrorCode(ErrorCode.SUCCESS);
                }
                else
                {
                    response.error.SetErrorCode(ErrorCode.NO_DATA);
                }
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
            }

            return Ok(response);
        }

        [HttpPost("page_list_showtime_by_movies")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<PageListShowtimesByMoviesDTO>), description: "GetPageListShowtimesByMovies Response")]
        public async Task<IActionResult> GetPageListShowtimesByMovies(PageListShowtimesByMoviesRequest request)
        {
            var response = new BaseResponseMessageItem<PageListShowtimesByMoviesDTO>();
            try
            {
                // Bước 1: Lọc các rạp có suất chiếu phù hợp và lấy dữ liệu cơ bản
                var cinemas = _context.Cinemas
                .Include(c => c.Screen)
                    .ThenInclude(s => s.Showtimes)
                .Include(c => c.Screen)
                    .ThenInclude(s => s.ScreenTypeUu)
                .Where(c => c.Screen.Any(screen => screen.Showtimes.Any(showtime =>
                    showtime.MoviesUuid == request.MoviesUuid &&
                    showtime.ShowDate == request.FindDate &&
                    showtime.LanguageType == request.LanguageType &&
                    showtime.Status == 1 &&
                    showtime.State == 0)))
                .ToList(); // Chuyển về bộ nhớ sau bước lọc cơ bản

                            // Bước 2: Thực hiện GroupBy và xử lý logic phức tạp phía client
                 var cinemaDTOs = cinemas.Select(cinema => new PageListShowtimesByMoviesDTO
                {
                    CinemaName = cinema.CinemaName,
                    Address = cinema.Address,
                    Location = cinema.Location,
                    Screens = cinema.Screen
                        .Where(screen => screen.Showtimes.Any(showtime =>
                            showtime.MoviesUuid == request.MoviesUuid &&
                            showtime.ShowDate == request.FindDate &&
                            showtime.LanguageType == request.LanguageType &&
                            showtime.Status == 1 &&
                            showtime.State == 0))
                        .Select(screen => new ScreenGroupDTO
                        {
                            ScreenTypeName = screen.ScreenTypeUu.Name,
                            Showtimes = screen.Showtimes
                                .Where(showtime =>
                                    showtime.MoviesUuid == request.MoviesUuid &&
                                    showtime.ShowDate == request.FindDate &&
                                    showtime.LanguageType == request.LanguageType &&
                                    showtime.Status == 1 &&
                                    showtime.State == 0)
                                .Select(showtime => new FormShowtimesByMoviesDTO
                                {
                                    StartTime = showtime.StartTime,
                                    EndTime = showtime.EndTime,
                                    LanguageType = showtime.LanguageType,
                                    ShowDate = showtime.ShowDate,
                                    Status = showtime.Status,
                                    Uuid = showtime.Uuid
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToList();
                cinemaDTOs.ForEach(cinema =>
                {
                    cinema.Screens = cinema.Screens
                        .GroupBy(screen => screen.ScreenTypeName)
                        .Select(group => new ScreenGroupDTO
                        {
                            ScreenTypeName = group.Key,
                            Showtimes = group.SelectMany(screen => screen.Showtimes).ToList()
                        })
                        .Where(screenGroup => screenGroup.Showtimes.Any()) // Loại bỏ màn hình không có suất chiếu
                        .ToList();
                });



                response.Data = cinemaDTOs;
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

        [HttpPost("page_list_showtime_by_cinema")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<PageListShowtimesByCinemaDTO>), description: "GetPageListShowtimesByCinema Response")]
        public async Task<IActionResult> GetPageListShowtimesByCinema(PageListShowtimesByCinemaRequest request)
        {
            var response = new BaseResponseMessageItem<PageListShowtimesByCinemaDTO>();
            try
            {
                var cinema = _context.Cinemas.Where(x => x.Uuid == request.CinemaUuid).FirstOrDefault();
                var validShowtimes = _context.Showtimes
                    .Include(x => x.MoviesUu)
                    .Include(x => x.ScreenUu).ThenInclude(x => x.ScreenTypeUu)
                    .Include(x => x.ScreenUu).ThenInclude(x => x.CinemaUu)
                    .Where(showtime =>
                        showtime.ScreenUu.CinemaUu.Uuid == request.CinemaUuid &&
                        showtime.ShowDate == request.FindDate &&
                        showtime.Status == 1 &&
                        showtime.State == 0)
                    .ToList();

                // 2. Lấy danh sách màn hình có suất chiếu hợp lệ
                var screensWithShowtimes = _context.Screen
                    .Where(screen => screen.Showtimes.Any(st => validShowtimes.Select(vs => vs.Uuid).Contains(st.Uuid)))
                    .ToList();

                // 3. Nhóm các suất chiếu theo thông tin phim
                var groupedShowtimesByMovies = validShowtimes
                    .GroupBy(showtime => new
                    {
                        MoviesUuid = showtime.MoviesUu.Uuid,
                        MoviesName = showtime.MoviesUu.Title,
                        Rated = showtime.MoviesUu.Rated
                    })
                    .ToList();

                // 4. Lấy thông tin thể loại phim
                var movieGenres = groupedShowtimesByMovies
                    .Select(group => new
                    {
                        group.Key.MoviesUuid,
                        Genre = _context.MoviesGenre
                            .Where(mg => mg.MoviesUuid == group.Key.MoviesUuid)
                            .Select(mg => new CategoryDTO
                            {
                                Uuid = mg.GenreUu.Uuid,
                                Name = mg.GenreUu.GenreName
                            })
                            .ToList()
                    })
                    .ToList();

                // 5. Nhóm suất chiếu theo loại màn hình
                var screenGroups = groupedShowtimesByMovies
                    .Select(group => new
                    {
                        group.Key.MoviesUuid,
                        Screens = group
                            .GroupBy(showtime => new
                            {
                                ScreenTypeName = showtime.ScreenUu.ScreenTypeUu.Name,
                                LanguageType = showtime.LanguageType == 1? "Phụ đề" : "Lồng tiếng",
                            }
                            
                            )
                            .Select(screenGroup => new ScreenGroupDTO
                            {
                                ScreenTypeName = screenGroup.Key.ScreenTypeName,
                                LanguageType = screenGroup.Key.LanguageType,
                                Showtimes = screenGroup.Select(showtime => new FormShowtimesByMoviesDTO
                                {
                                    ShowDate = showtime.ShowDate,
                                    StartTime = showtime.StartTime,
                                    EndTime = showtime.EndTime,
                                    LanguageType = showtime.LanguageType,
                                    Status = showtime.Status,
                                    Uuid = showtime.Uuid
                                }).ToList()
                            })
                            .ToList()
                    })
                    .ToList();

                // 6. Tạo danh sách DTO hoàn chỉnh
                var cinemaShowtimes = groupedShowtimesByMovies
                    .Select(group => new PageListShowtimesByCinemaDTO
                    {
                        MoviesUuid = group.Key.MoviesUuid,
                        MoviesName = group.Key.MoviesName,
                        ImageUrl = _context.Images.Where(x => group.Key.MoviesUuid == x.OwnerUuid && x.Status == 1).Select(x => x.Path).FirstOrDefault(),
                        Rated = group.Key.Rated,

                        Genre = movieGenres
                            .FirstOrDefault(mg => mg.MoviesUuid == group.Key.MoviesUuid)?.Genre,
                        Screens = screenGroups
                            .FirstOrDefault(sg => sg.MoviesUuid == group.Key.MoviesUuid)?.Screens
                    })
                    .ToList();
                response.Data = cinemaShowtimes;
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

    }
}
