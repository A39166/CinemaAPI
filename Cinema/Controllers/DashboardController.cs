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
                    TotalRevenueByMonth = _context.Bill.Where(b => b.TimeCreated >= startOfMonth && b.TimeCreated <= endOfMonth).Sum(b => b.PayPrice),
                    TotalRevenueByYear = _context.Bill.Where(b => b.TimeCreated.Year == DateTime.Now.Year).Sum(b => b.PayPrice)
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
        [HttpPost("dashboard_chart")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<DashboardChartDTO>), description: "DashboardChart Response")]
        public async Task<IActionResult> DashboardChart()
        {
            var response = new BaseResponseMessageItem<DashboardChartDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var currentYear = DateTime.Now.Year;
                response.Data = Enumerable.Range(1, 12).Select(month => new DashboardChartDTO
                {
                    Month = month,
                    Year = currentYear,
                    TotalRevenue = _context.Bill.Where(b => b.TimeCreated.Year == currentYear && b.TimeCreated.Month == month && b.State == 1) 
                                                .Sum(b => b.PayPrice) 
                }).ToList();
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
        /*[HttpPost("dashboard_list_revenue_by_movies")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<RevenueByMoviesDTO>), description: "GetCouponDetail Response")]
        public async Task<IActionResult> GetCouponDetail()
        {
            var response = new BaseResponseMessageItem<RevenueByMoviesDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                response.Data = _context.Mo
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
        }*/
        
    }
}
