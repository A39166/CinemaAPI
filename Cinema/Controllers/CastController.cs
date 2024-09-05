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
    [SwaggerTag("Director Controller")]
    public class CastController : BaseController
    {
        private readonly ILogger<CastController> _logger;
        private readonly DBContext _context;

        public CastController(DBContext context, ILogger<CastController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("upsert_cast")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpsertCast Response")]
        public async Task<IActionResult> UpsertCast(UpsertCastRequest request)
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
                    
                    var cast = new Cast()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        CastName = request.CastName,
                        Birthday = request.Birthday,
                        Description = request.Description,
                        TimeCreated = DateTime.Now,
                        Status = 1,
                    };
                    _context.Cast.Add(cast);
                    _context.SaveChanges();
                    if (!string.IsNullOrEmpty(request.ImagesUuid))
                    {
                        var image = _context.Images.FirstOrDefault(img => img.Uuid == request.ImagesUuid);
                        if (image != null)
                        {
                            image.OwnerUuid = cast.Uuid;
                            image.OwnerType = "cast";
                            image.Status = 1;
                            _context.Images.Update(image);
                            _context.SaveChanges();
                        }
                    }
                }
                else
                //cập nhập dữ liệu
                {
                    var cast = _context.Cast.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                    if (cast != null)
                    {
                        cast.CastName = request.CastName;
                        cast.Birthday = request.Birthday;
                        cast.Description = request.Description;
                        cast.Status = 1;
                        _context.SaveChanges();
                        if (!string.IsNullOrEmpty(request.ImagesUuid))
                        {
                            var newimage = _context.Images.SingleOrDefault(img => img.Uuid == request.ImagesUuid);
                            if (newimage != null)
                            {
                                var oldImage = _context.Images.SingleOrDefault(img => img.OwnerUuid == cast.Uuid);
                                if (oldImage != null)
                                {
                                    _context.Images.Remove(oldImage);
                                }
                                newimage.OwnerUuid = cast.Uuid;
                                newimage.OwnerType = "cast";
                                newimage.Status = 1;
                                _context.Images.Update(newimage);
                                _context.SaveChanges();
                            }
                        }
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
        [HttpPost("page_list_cast")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<CastDTO>), description: "GetPageListCast Response")]
        public async Task<IActionResult> GetPageListCast(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<CastDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var query = _context.Cast;
                var lstCast = query.ToList();
                var totalcount = query.Count();

                if (lstCast != null && lstCast.Count > 0)
                {
                    var result = lstCast.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<CastDTO>();
                    }
                    foreach (var cast in result)
                    {
                        var convertItemDTO = new CastDTO()
                        {
                            Uuid = cast.Uuid,
                            CastName = cast.CastName,
                            Birthday = cast.Birthday,
                            Description = cast.Description,
                            ImageUrl = _context.Images.Where(x => cast.Uuid == x.OwnerUuid).Select(x => x.Path).FirstOrDefault(),
                            TimeCreated = cast.TimeCreated,
                            Status = cast.Status,
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
                        ImageUrl = _context.Images.Where(x => castdetail.Uuid == x.OwnerUuid).Select(x => x.Path).FirstOrDefault(),
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
