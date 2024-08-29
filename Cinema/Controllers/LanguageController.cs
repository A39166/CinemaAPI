/*using CinemaAPI.Databases.CinemaDB;
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
    [ApiVersion("1")]
    [SwaggerTag("Region Controller")]
    public class RegionController : BaseController
    {
        private readonly ILogger<RegionController> _logger;
        private readonly DBContext _context;

        public RegionController(DBContext context, ILogger<RegionController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("upsert_region")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpsertRegion Response")]
        public async Task<IActionResult> UpsertRegion(UpsertRegionRequest request)
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
                    var regionName = _context.Region.Where(d => d.RegionName.Trim().ToLower() == request.RegionName.Trim().ToLower()).FirstOrDefault();
                    if (regionName != null)
                    {
                        response.error.SetErrorCode(ErrorCode.DUPLICATE_REGION);
                        return BadRequest(response);
                    }
                    else
                    {
                        var region = new Region()
                        {
                            Uuid = Guid.NewGuid().ToString(),
                            RegionName = request.RegionName,
                            Status = 1,
                        };
                        _context.Region.Add(region);
                        _context.SaveChanges();
                    }
                    
                }
                else
                //cập nhập dữ liệu
                {
                    var region = _context.Region.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                    if (region != null)
                    {
                        region.RegionName = request.RegionName;
                        region.Status = 1;
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
        [HttpPost("page_list_region")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<RegionDTO>), description: "GetPageListRegion Response")]
        public async Task<IActionResult> GetPageListRegion(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<RegionDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var query = _context.Region;
                var lstRegion = query.ToList();
                var totalcount = query.Count();

                if (lstRegion != null && lstRegion.Count > 0)
                {
                    var result = lstRegion.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<RegionDTO>();
                    }
                    foreach (var region in result)
                    {
                        var convertItemDTO = new RegionDTO()

                        {
                            Uuid = region.Uuid,
                            RegionName = region.RegionName,
                            Status = region.Status,
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
        [HttpPost("region_detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(RegionDTO), description: "GetRegionDetail Response")]
        public async Task<IActionResult> GetRegionDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<RegionDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                //TODO: Write code late

                var regiondetail = _context.Region.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                if (regiondetail != null)
                {
                    response.Data = new RegionDTO()
                    {
                        Uuid = regiondetail.Uuid,
                        RegionName = regiondetail.RegionName,
                        Status = regiondetail.Status,
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
        [HttpPost("update_region_status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateRegionStatus Response")]
        public async Task<IActionResult> UpdateRegionStatus(UpdateStatusRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                //TODO: Write code late

                var regionstatus = _context.Region.Where(x => x.Uuid == request.Uuid).SingleOrDefault();

                if (regionstatus != null)
                {
                    regionstatus.Status = request.Status;
   
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
*/