using CinemaAPI.Databases.CinemaDB;
using CinemaAPI.Enums;
using CinemaAPI.Extensions;
using CinemaAPI.Models.BaseRequest;
using CinemaAPI.Models.DataInfo;
using CinemaAPI.Models.Request;
using CinemaAPI.Models.Response;
using CinemaAPI.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace CinemaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class GenreController : BaseController
    {
        private readonly ILogger<GenreController> _logger;
        private readonly DBContext _context;

        public GenreController(DBContext context, ILogger<GenreController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("upsert_genre")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "Upsert Genre")]
        public async Task<IActionResult> UpsertGenre(UpsertGenreRequest request)
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
                    var genreName = _context.Genre.Where(d => d.GenreName.Trim().ToLower() == request.GenreName.Trim().ToLower()).FirstOrDefault();
                    if (genreName != null)
                    {
                        response.error.SetErrorCode(ErrorCode.DUPLICATE_GENRE);
                        return BadRequest(response);
                    }
                    else
                    {
                        var genre = new Genre()
                        {
                            Uuid = Guid.NewGuid().ToString(),
                            GenreName = request.GenreName,
                            Status = 1,
                        };
                        _context.Genre.Add(genre);
                        _context.SaveChanges();
                    }
                    
                }
                else
                //cập nhập dữ liệu
                {
                    var genre = _context.Genre.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                    if (genre != null)
                    {
                        genre.GenreName = request.GenreName;
                        genre.Status = 1;
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
        [HttpPost("page_list_genre")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<GenreDTO>), description: "Upsert Genre")]
        public async Task<IActionResult> GetPageListGenre(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<GenreDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var query = _context.Genre;
                var lstGenre = query.ToList();
                var totalcount = query.Count();

                if (lstGenre != null && lstGenre.Count > 0)
                {
                    var result = lstGenre.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<GenreDTO>();
                    }
                    foreach (var genre in result)
                    {
                        var convertItemDTO = new GenreDTO()

                        {
                            Uuid = genre.Uuid,
                            GenreName = genre.GenreName,
                            Status = genre.Status,
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
