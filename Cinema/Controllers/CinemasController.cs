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
using System.Net;

namespace CinemaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Cast Controller")]
    public class CinemasController : BaseController
    {
        private readonly ILogger<CinemasController> _logger;
        private readonly DBContext _context;

        public CinemasController(DBContext context, ILogger<CinemasController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("upsert_cinema")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpsertCinema Response")]
        public async Task<IActionResult> UpsertCinema(UpsertCinemaRequest request)
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

                    var cinema = new Cinemas()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        CinemaName = request.CinemaName,
                        Address = request.Address,
                        Location = request.Location,
                        TimeCreated = DateTime.Now,
                        Status = 1,
                    };
                    _context.Cinemas.Add(cinema);
                    _context.SaveChanges();
                }
                else
                //cập nhập dữ liệu
                {
                    var cinema = _context.Cinemas.Where(x => x.Uuid == request.Uuid).FirstOrDefault();
                    if (cinema != null)
                    {
                        cinema.CinemaName = request.CinemaName;
                        cinema.Address = request.Address;
                        cinema.Location = request.Location;
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
        [HttpPost("page_list_cinema")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<CinemasDTO>), description: "GetPageListCinema Response")]
        public async Task<IActionResult> GetPageListCinema(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<CinemasDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var query = _context.Cinemas;
                var lstCinema = query.ToList();
                var totalcount = query.Count();

                if (lstCinema != null && lstCinema.Count > 0)
                {
                    var result = lstCinema.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<CinemasDTO>();
                    }
                    foreach (var cinemas in result)
                    {
                        var convertItemDTO = new CinemasDTO()
                        {
                            Uuid = cinemas.Uuid,
                            CinemaName = cinemas.CinemaName,
                            Address = cinemas.Address,
                            Location = cinemas.Location,
                            TimeCreated = cinemas.TimeCreated,
                            Status = cinemas.Status,
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
        [HttpPost("cinema_detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(CinemasDTO), description: "GetCinemaDetail Response")]
        public async Task<IActionResult> GetCinemaDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<CinemasDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var cinemas = _context.Cinemas.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                if (cinemas != null)
                {
                    response.Data = new CinemasDTO()
                    {
                        Uuid = cinemas.Uuid,
                        CinemaName = cinemas.CinemaName,
                        Address = cinemas.Address,
                        Location = cinemas.Location,
                        TimeCreated = cinemas.TimeCreated,
                        Status = cinemas.Status,
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
        [HttpPost("update_cinema_status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateCinemaStatus Response")]
        public async Task<IActionResult> UpdateCinemaStatus(UpdateStatusRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var cinemastatus = _context.Cinemas.Where(x => x.Uuid == request.Uuid).SingleOrDefault();

                if (cinemastatus != null)
                {
                    cinemastatus.Status = request.Status;

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
