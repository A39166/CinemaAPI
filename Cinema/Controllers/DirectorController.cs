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

namespace CinemaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Cast Controller")]
    public class DirectorController : BaseController
    {
        private readonly ILogger<DirectorController> _logger;
        private readonly DBContext _context;

        public DirectorController(DBContext context, ILogger<DirectorController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("upsert_director")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpsertDirector Response")]
        public async Task<IActionResult> UpsertDirector(UpsertDirectorRequest request)
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
                    
                    var director = new Director()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        DirectorName = request.DirectorName,
                        Birthday = request.Birthday,
                        Description = request.Description,
                        TimeCreated = DateTime.Now,
                        Status = 1,
                    };
                    _context.Director.Add(director);
                    _context.SaveChanges();
                    if (!string.IsNullOrEmpty(request.ImagesUuid))
                    {
                        var image = _context.Images.FirstOrDefault(img => img.Uuid == request.ImagesUuid);
                        if (image != null)
                        {
                            image.OwnerUuid = director.Uuid;
                            image.OwnerType = "director";
                            image.Status = 1;
                            _context.Images.Update(image);
                            _context.SaveChanges();
                        }
                    }
                }
                else
                //cập nhập dữ liệu
                {
                    var director = _context.Director.Where(x => x.Uuid == request.Uuid).FirstOrDefault();
                    if (director != null)
                    {
                        director.DirectorName = request.DirectorName;
                        director.Birthday = request.Birthday;
                        director.Description = request.Description;
                        director.Status = 1;
                        _context.SaveChanges();
                        if (!string.IsNullOrEmpty(request.ImagesUuid))
                        {
                            var oldImageUuid = _context.Images.Where(x => x.OwnerUuid == request.Uuid).Select(u => u.Path).FirstOrDefault();
                            if (oldImageUuid != request.ImagesUuid)
                            {
                                var newimage = _context.Images.FirstOrDefault(img => img.Uuid == request.ImagesUuid);
                                if (newimage != null)
                                {
                                    var oldImage = _context.Images.FirstOrDefault(img => img.OwnerUuid == request.Uuid && img.Status == 1);
                                    if (oldImage != null)
                                    {
                                        oldImage.Status = 0;
                                        _context.Images.Update(oldImage);
                                    }
                                    newimage.OwnerUuid = director.Uuid;
                                    newimage.OwnerType = "director";
                                    newimage.Status = 1;
                                    _context.Images.Update(newimage);
                                    _context.SaveChanges();
                                }

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
        [HttpPost("page_list_director")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<DirectorDTO>), description: "GetPageListDirector Response")]
        public async Task<IActionResult> GetPageListDirector(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<DirectorDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var query = _context.Director;
                var lstDirector = query.ToList();
                var totalcount = query.Count();

                if (lstDirector != null && lstDirector.Count > 0)
                {
                    var result = lstDirector.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<DirectorDTO>();
                    }
                    foreach (var director in result)
                    {
                        var convertItemDTO = new DirectorDTO()
                        {
                            Uuid = director.Uuid,
                            DirectorName = director.DirectorName,
                            Birthday = director.Birthday,
                            Description = director.Description,
                            ImageUrl = _context.Images.Where(x => director.Uuid == x.OwnerUuid && x.Status == 1).Select(x => x.Path).FirstOrDefault(),
                            TimeCreated = director.TimeCreated,
                            Status = director.Status,
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
        [HttpPost("director_detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(DirectorDTO), description: "GetDirectorDetail Response")]
        public async Task<IActionResult> GetDirectorDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<DirectorDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                //TODO: Write code late

                var directordetail = _context.Director.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                if (directordetail != null)
                {
                    response.Data = new DirectorDTO()
                    {
                        Uuid = directordetail.Uuid,
                        DirectorName = directordetail.DirectorName,
                        Birthday = directordetail.Birthday,
                        Description = directordetail.Description,
                        ImageUrl = _context.Images.Where(x => directordetail.Uuid == x.OwnerUuid).Select(x => x.Path).FirstOrDefault(),
                        TimeCreated = directordetail.TimeCreated,
                        Status = directordetail.Status,
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
        [HttpPost("update_director_status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateDirectorStatus Response")]
        public async Task<IActionResult> UpdateDirectorStatus(UpdateStatusRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var directorstatus = _context.Director.Where(x => x.Uuid == request.Uuid).SingleOrDefault();

                if (directorstatus != null)
                {
                    directorstatus.Status = request.Status;
                    var img = _context.Images.Where(x => x.OwnerUuid == request.Uuid).SingleOrDefault();
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
