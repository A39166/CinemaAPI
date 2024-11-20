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
using System.Net.Sockets;

namespace CinemaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Coupon Controller")]
    public class CouponController : BaseController
    {
        private readonly ILogger<CouponController> _logger;
        private readonly DBContext _context;

        public CouponController(DBContext context, ILogger<CouponController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("upsert_coupon")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpsertCoupon Response")]
        public async Task<IActionResult> UpsertCoupon(UpsertCouponRequest request)
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
                    
                    var coupon = new Coupon()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        Code = request.Code,
                        Quantity = request.Quantity,
                        Used = 0,
                        Discount = request.Discount,
                        StartDate = request.StartDate,
                        EndDate = request.EndDate,
                        TimeCreated = DateTime.Now,
                        Status = request.Status,
                    };
                    _context.Coupon.Add(coupon);
                    _context.SaveChanges();
                }
                else
                //cập nhập dữ liệu
                {
                    var coupon = _context.Coupon.Where(x => x.Uuid == request.Uuid).FirstOrDefault();
                    if (coupon == null)
                    {
                        throw new ErrorException(ErrorCode.NOT_FOUND);
                    }
                    if (coupon != null)
                    {
                        coupon.Code = request.Code;
                        coupon.Quantity = request.Quantity;
                        coupon.Discount = request.Discount;
                        coupon.StartDate = request.StartDate;
                        coupon.EndDate = request.EndDate;
                        coupon.Status = request.Status;
                        _context.Coupon.Update(coupon);
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
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }
        [HttpPost("page_list_coupon")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<CouponDTO>), description: "GetPageListCoupon Response")]
        public async Task<IActionResult> GetPageListCoupon(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<CouponDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var query = _context.Coupon;
                var lstCoupon = query.Where(x => x.Status != 0).ToList();
                var totalcount = query.Count();

                if (lstCoupon != null && lstCoupon.Count > 0)
                {
                    var result = lstCoupon.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<CouponDTO>();
                    }
                    foreach (var coupon in result)
                    {
                        var convertItemDTO = new CouponDTO()
                        {
                            Uuid = coupon.Uuid,
                            Code = coupon.Code,
                            Quantity = coupon.Quantity,
                            Used = coupon.Used,
                            Discount = coupon.Discount,
                            StartDate = coupon.StartDate, 
                            EndDate = coupon.EndDate,

                            Status = coupon.Status,
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
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }
        [HttpPost("coupon_detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(CouponDTO), description: "GetCouponDetail Response")]
        public async Task<IActionResult> GetCouponDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<CouponDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var coupondetail = _context.Coupon.Where(x => x.Uuid == request.Uuid && x.Status != 0).SingleOrDefault();
                if (coupondetail == null)
                {
                    throw new ErrorException(ErrorCode.NOT_FOUND);
                }
                if (coupondetail != null)
                {
                    response.Data = new CouponDTO()
                    {
                        Uuid = coupondetail.Uuid,
                        Code = coupondetail.Code,
                        Quantity = coupondetail.Quantity,
                        Discount = coupondetail.Discount,
                        StartDate = coupondetail.StartDate,
                        EndDate = coupondetail.EndDate,
                        Status = coupondetail.Status,
                    };

                }
                return Ok(response);
            }
            catch (ErrorException ex)
            {
                response.error.SetErrorCode(ex.Code);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);
                return BadRequest(response);
            }
        }
        [HttpPost("update_coupon_status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateCouponStatus Response")]
        public async Task<IActionResult> UpdateCouponStatus(UpdateStatusRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var couponstatus = _context.Coupon.Where(x => x.Uuid == request.Uuid && x.Status != 0).SingleOrDefault();

                if (couponstatus != null)
                {
                    couponstatus.Status = request.Status;
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
