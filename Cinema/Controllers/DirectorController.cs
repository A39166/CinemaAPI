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

namespace CinemaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Director Controller")]
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
                        DirectorBirth = request.DirectorBirth,
                        DirectorDescription = request.DirectorDescription,
                        Status = 1,
                    };
                    _context.Director.Add(director);
                    _context.SaveChanges();
                }
                else
                //cập nhập dữ liệu
                {
                    var director = _context.Director.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                    if (director != null)
                    {
                        director.DirectorName = request.DirectorName;
                        director.DirectorBirth = request.DirectorBirth;
                        director.DirectorDescription = request.DirectorDescription;
                        director.Status = 1;
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
                            DirectorBirth = director.DirectorBirth,
                            DirectorDescription = director.DirectorDescription,
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
    }
}
