﻿using CinemaAPI.Models.Request;
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
    [SwaggerTag("Dashboard Controller")]
    public class DashboardController : BaseController
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly DBContext _context;

        public DashboardController(DBContext context, ILogger<DashboardController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("dashboard_overview")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessage<DashboardOverviewDTO>), description: "DashboardOverview Response")]
        public async Task<IActionResult> DasboardOverview()
        {
            var response = new BaseResponseMessage<DashboardOverviewDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); // Ngày đầu tháng
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                response.Data = new DashboardOverviewDTO()
                {
                    TodayRevenue = _context.Bill.Where(x => x.TimeCreated == DateTime.Now).Sum(x => x.PayPrice),
                    TotalTicketSell = _context.Bill.Include(b => b.Booking).Where(x => x.TimeCreated >= startOfMonth && x.TimeCreated <= endOfMonth)
                                                    .Sum(b => b.Booking.Count()),
                    TotalRevenue = _context.Bill.Where(b => b.TimeCreated >= startOfMonth && b.TimeCreated <= endOfMonth).Sum(b => b.PayPrice)
                };
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
