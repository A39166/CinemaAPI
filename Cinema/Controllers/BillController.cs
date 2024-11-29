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

namespace CinemaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Bill Controller")]
    public class BillController : BaseController
    {
        private readonly ILogger<BillController> _logger;
        private readonly DBContext _context;

        public BillController(DBContext context, ILogger<BillController> logger)
        {

            _context = context;
            _logger = logger;
        }
        
        [HttpPost("page_list_bill_admin")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<PageListBillAdminDTO>), description: "GetPageListBillAdmin Response")]
        public async Task<IActionResult> GetPageListBillAdmin(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<PageListBillAdminDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {

                var lstBill = _context.Bill.Include(p => p.ShowtimeUu).ThenInclude(m => m.MoviesUu)
                                             .Include(p => p.ShowtimeUu).ThenInclude(s => s.ScreenUu).ThenInclude(c => c.CinemaUu)
                                             .Where(x => x.Status == 1)
                                             .ToList();

                var totalcount = lstBill.Count();

                if (lstBill != null && lstBill.Count > 0)
                {
                    var result = lstBill.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<PageListBillAdminDTO>();
                    }
                    foreach (var bill in result)
                    {
                        var convertItemDTO = new PageListBillAdminDTO()
                        {
                            Uuid = bill.Uuid,
                            Code = bill.Code,
                            MovieName = bill.ShowtimeUu.MoviesUu.Title,
                            ScreenName = bill.ShowtimeUu.ScreenUu.ScreenName,
                            CinemaName = bill.ShowtimeUu.ScreenUu.CinemaUu.CinemaName,
                            State = bill.State,
                            PayPrice = bill.PayPrice,
                            ShowDate = bill.ShowtimeUu.ShowDate,
                            StartTime = bill.ShowtimeUu.StartTime,
                            EndTime = bill.ShowtimeUu.EndTime,
                            TimeCreated = bill.TimeCreated,
                            Status = bill.Status,
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
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }


        [HttpPost("screen_detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(ScreenDTO), description: "GetScreenDetail Response")]
        public async Task<IActionResult> GetScreenDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<ScreenDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                //TODO: Write code late

                var screendetail = _context.Screen.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                if (screendetail != null)
                {
                    response.Data = new ScreenDTO()
                    {
                        Uuid = screendetail.Uuid,
                        CinemaUuid = screendetail.CinemaUuid,
                        ScreenName = screendetail.ScreenName,
                        ScreenType = _context.ScreenType.Where(x => x.Uuid == screendetail.ScreenTypeUuid).Select(p => p.Type).FirstOrDefault(),
                        Capacity = screendetail.Capacity,
                        Row = screendetail.Row,
                        Collumn = screendetail.Collumn,
                        TimeCreated = screendetail.TimeCreated,
                        Status = screendetail.Status,
                    };
                }
                return Ok(response);
            }
            catch (ErrorException ex)
            {
                response.error.SetErrorCode(ex.Code);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);
                return BadRequest(response);
            }
        }
        [HttpPost("page_list_bill_client")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<PageListBillClientDTO>), description: "GetPageListBillClient Response")]
        public async Task<IActionResult> GetPageListBillClient(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<PageListBillClientDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {

                var lstBill = _context.Bill.Include(p => p.ShowtimeUu).ThenInclude(m => m.MoviesUu)
                                             .Include(p => p.ShowtimeUu).ThenInclude(s => s.ScreenUu).ThenInclude(c => c.CinemaUu)
                                             
                                             .Where(x => x.Status == 1 && x.UserUuid == validToken.UserUuid)
                                             .ToList();

                var totalcount = lstBill.Count();

                if (lstBill != null && lstBill.Count > 0)
                {
                    var result = lstBill.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<PageListBillClientDTO>();
                    }
                    foreach (var bill in result)
                    {
                        var convertItemDTO = new PageListBillClientDTO()
                        {
                            Uuid = bill.Uuid,
                            Code = bill.Code,
                            MovieName = bill.ShowtimeUu.MoviesUu.Title,                           
                            PayPrice = bill.PayPrice,
                            State = bill.State,
                            TimeCreated = bill.TimeCreated,
                            Status = bill.Status,
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
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }

        [HttpPost("bill_detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(BillDetailCLientDTO), description: "GetBillDetail Response")]
        public async Task<IActionResult> GetBillDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<BillDetailCLientDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                //TODO: Write code late

                var bill = _context.Bill.Include(s => s.ShowtimeUu).ThenInclude(m => m.MoviesUu)
                                                .Include(s => s.ShowtimeUu).ThenInclude(m => m.ScreenUu).ThenInclude(m => m.CinemaUu)
                                                .Include(s => s.ShowtimeUu).ThenInclude(m => m.ScreenUu).ThenInclude(m => m.ScreenTypeUu)
                                                .Include(b => b.Booking).ThenInclude(s => s.SeatUu)
                                                .Include(c => c.CouponUu)
                                                .Include(b => b.Booking).ThenInclude(t => t.TicketUu)
                                                .Include(bcb => bcb.BillCombo).ThenInclude(cb => cb.ComboUu)
                                                .Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                if (bill != null)
                {
                    response.Data = new BillDetailCLientDTO()
                    {
                        Uuid = bill.Uuid,
                        Movie = new ShortMoviesDTO
                        {
                            Uuid = bill.ShowtimeUu.MoviesUuid,
                            Title = bill.ShowtimeUu.MoviesUu.Title,
                            Rated = bill.ShowtimeUu.MoviesUu.Rated,
                        },
                        Screen = new ShortScreenDTO
                        {
                            Uuid = bill.ShowtimeUu.ScreenUu.Uuid,
                            ScreenName = bill.ShowtimeUu.ScreenUu.ScreenName,
                            ScreenTypeName = bill.ShowtimeUu.ScreenUu.ScreenTypeUu.Name,
                        },
                        Cinema = new ShortCinemaDTO
                        {
                            Uuid = bill.ShowtimeUu.ScreenUu.CinemaUu.Uuid,
                            CinemaName = bill.ShowtimeUu.ScreenUu.CinemaUu.CinemaName,
                            Address = bill.ShowtimeUu.ScreenUu.CinemaUu.Address,
                            Status = bill.ShowtimeUu.ScreenUu.CinemaUu.Status,
                        },
                        Seat = bill.Booking.Select(b => new SeatBillDTO
                        {
                            Uuid = b.SeatUu.Uuid,
                            SeatCode = b.SeatUu.SeatCode,
                            Price = b.TicketUu.Price,
                        }).ToList(),
                        Combo = bill.BillCombo.Select(cb => new ComboForBill
                        {
                            Uuid = cb.ComboUu.Uuid,
                            ComboName = cb.ComboUu.ComboName,
                            Price = cb.ComboUu.Price,
                        }).ToList(),
                        ShowDate = bill.ShowtimeUu.ShowDate,
                        StartTime = bill.ShowtimeUu.StartTime,
                        EndTime = bill.ShowtimeUu.EndTime,
                        CouponUuid = bill.CouponUuid,
                        LanguageTypeName = bill.ShowtimeUu.LanguageType == 1 ? "Phụ đề" : "Lồng tiếng",
                        TotalComboPrice = bill.TotalComboPrice,
                        TotalSeatPrice = bill.TotalSeatPrice,
                        PayPrice = bill.PayPrice,
                        QRPath = bill.QrPath,
                        Status = bill.Status,
                    };
                }
                return Ok(response);
            }
            catch (ErrorException ex)
            {
                response.error.SetErrorCode(ex.Code);
                return BadRequest(response);
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
