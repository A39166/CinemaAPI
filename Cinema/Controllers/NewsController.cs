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

namespace CinemaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("News Controller")]
    public class NewsController : BaseController
    {
        private readonly ILogger<NewsController> _logger;
        private readonly DBContext _context;

        public NewsController(DBContext context, ILogger<NewsController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("upsert_news")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpsertScreen Response")]
        public async Task<IActionResult> UpsertScreen(UpsertScreenRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                if (string.IsNullOrEmpty(request.Uuid))
                {
                    
                    var screen = new Screen()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        ScreenName = request.ScreenName,
                        CinemaUuid = request.CinemaUuid,
                        ScreenTypeUuid = request.ScreenTypeUuid,
                        Capacity = request.Capacity,
                        Row = request.Rows,
                        Collumn = request.Columns,
                        TimeCreated = DateTime.Now,
                        Status = 1,
                    };
                    _context.Screen.Add(screen);
                    _context.SaveChanges();
                    
                }
                else
                //cập nhập dữ liệu
                {
                    var screen = _context.Screen.Where(x => x.Uuid == request.Uuid).FirstOrDefault();
                    if (screen != null)
                    {
                        screen.ScreenName = request.ScreenName;
                        screen.CinemaUuid = request.CinemaUuid;
                        screen.ScreenTypeUuid = request.ScreenTypeUuid;
                        screen.Capacity = request.Capacity;
                        screen.Row = request.Rows;
                        screen.Collumn = request.Columns;
                        screen.Status = 1;

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
        [HttpPost("cast_detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(CastDTO), description: "GetCastDetail Response")]
        public async Task<IActionResult> GetCastDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<CastDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                //TODO: Write code late

                var castdetail = _context.Cast.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                if (castdetail != null)
                {
                    response.Data = new CastDTO()
                    {
                        Uuid = castdetail.Uuid,
                        CastName = castdetail.CastName,
                        Birthday = castdetail.Birthday,
                        Description = castdetail.Description,
                        ImageUrl = _context.Images.Where(x => castdetail.Uuid == x.OwnerUuid && x.Status == 1).Select(x => x.Path).FirstOrDefault(),
                        TimeCreated = castdetail.TimeCreated,
                        Status = castdetail.Status,
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
        [HttpPost("update_cast_status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateCastStatus Response")]
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
                var caststatus = _context.Cast.Where(x => x.Uuid == request.Uuid).SingleOrDefault();

                if (caststatus != null)
                {
                    caststatus.Status = request.Status;
                    var img = _context.Images.Where(x => x.OwnerUuid == request.Uuid && x.Status == 1).SingleOrDefault();
                    if (img != null)
                    {
                        img.Status = request.Status;
                    }
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
