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
using VNPAY_CS_ASPX;

namespace CinemaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Booking Controller")]
    public class BookingController : BaseController
    {
        private readonly ILogger<BookingController> _logger;
        private readonly DBContext _context;

        public BookingController(DBContext context, ILogger<BookingController> logger)
        {

            _context = context;
            _logger = logger;
        }

        [HttpPost("create_bill")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "CreateBill Response")]
        public async Task<IActionResult> CreateBill(CreateBillRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {

                var bill = new Bill()
                {
                    Uuid = Guid.NewGuid().ToString(),
                    UserUuid = validToken.UserUuid,
                    ShowtimeUuid = request.ShowtimeUuid,
                    CouponUuid = request.CouponUuid ?? null,
                    TotalPrice = request.TotalPrice,
                    PayPrice = request.PayPrice,
                    State = 1,
                    TimeCreated = DateTime.Now,
                    Status = 1,
                };
                bill.Code = "PCB" + bill.Id;
                _context.Bill.Add(bill);
                foreach (var seat in request.Seats)
                {
                    var newSeat = new Booking()
                    {
                        BillUuid = bill.Uuid,
                        SeatUuid = seat.SeatUuid,
                        TicketUuid = seat.SeatPriceUuid,
                        Status = 1
                    };
                    _context.Booking.Add(newSeat);
                }
                foreach (var combo in request.Combo)
                {
                    var newCombo = new BillCombo()
                    {
                        BillUuid = bill.Uuid,
                        ComboUuid = combo,
                        Status = 1
                    };
                    _context.BillCombo.Add(newCombo);
                }
                _context.SaveChanges();
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
        [HttpPost("showtime_detail_for_booking")]
        [SwaggerResponse(statusCode: 200, type: typeof(ShowtimeClientDTO), description: "GetShowtimeDetailClient Response")]
        public async Task<IActionResult> GetShowtimeDetailClient(UuidRequest request)
        {
            var response = new BaseResponseMessage<ShowtimeClientDTO>();
            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var st = _context.Showtimes.Include(x => x.ScreenUu)
                    .Where(x => x.Uuid == request.Uuid).FirstOrDefault();
                /*var currentTime = DateTime.Now;
                var startTime = DateOnly.FromDateTime(currentTime).ToDateTime(st.StartTime);

                if ((startTime - currentTime).TotalMinutes <= 5)
                {
                    throw new ErrorException(ErrorCode.BOOKING_NOT_ALLOWED);
                }*/
                if (st != null)
                {
                    var movies = _context.Movies.Where(x => x.Uuid == st.MoviesUuid).FirstOrDefault();
                    var scr = _context.Screen.Where(x => x.Uuid == st.ScreenUuid).FirstOrDefault();
                    var scrtype = _context.ScreenType.Where(x => x.Uuid == scr.ScreenTypeUuid).FirstOrDefault();
                    response.Data = new ShowtimeClientDTO()
                    {
                        Uuid = st.Uuid,
                        Movies = new ShortMoviesDTO
                        {
                            Uuid = movies.Uuid,
                            Title = movies.Title,
                            Rated = movies.Rated
                        },
                        Screen = new ShortScreenDTO
                        {
                            Uuid = scr.Uuid,
                            ScreenName = scr.ScreenName,
                            ScreenTypeName = scrtype.Name,
                        },
                        LanguageType = st.LanguageType,
                        ShowDate = st.ShowDate,
                        StartTime = st.StartTime,
                        EndTime = st.EndTime,
                        Status = st.Status,
                    };
                }
                else
                {
                    throw new ErrorException(ErrorCode.NOT_FOUND);
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

        [HttpPost("get_list_seat_for_booking")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessage<ScreenSeatForBookingDTO>), description: "GetListSeatForBooking Response")]
        public async Task<IActionResult> GetListSeat(UuidRequest request)
        {
            var response = new BaseResponseMessage<ScreenSeatForBookingDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var st = _context.Showtimes.Include(s => s.ScreenUu).ThenInclude(s => s.ScreenTypeUu)
                    .Where(x => x.Uuid == request.Uuid).FirstOrDefault();
                var dateState = (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            ? 2
            : 1;
                var ScreenSeatDTO = _context.Screen.Where(x => x.Uuid == st.ScreenUuid && x.Status == 1)
                    .Select(screen => new ScreenSeatForBookingDTO
                    {
                        Row = screen.Row,
                        Collumn = screen.Collumn,
                        Seats = screen.Seat.Where(s => s.ScreenUuid == screen.Uuid).Select(seat => new SeatForBookingDTO
                        {
                            Uuid = seat.Uuid,
                            SeatCode = seat.SeatCode,
                            SeatType = _context.SeatType.Where(t => t.Uuid == seat.SeatTypeUuid).Select(t => t.Type).FirstOrDefault(),
                            Price = _context.Ticket.Where(p => p.ScreenTypeUuid == st.ScreenUu.ScreenTypeUu.Uuid &&
                                                          p.SeatTypeUuid == seat.SeatTypeUuid &&
                                                          p.DateState == dateState).Select(t => t.Price).FirstOrDefault(),
                            TicketPriceUuid = _context.Ticket.Where(p => p.ScreenTypeUuid == st.ScreenUu.ScreenTypeUu.Uuid &&
                                                          p.SeatTypeUuid == seat.SeatTypeUuid &&
                                                          p.DateState == dateState).Select(t => t.Uuid).FirstOrDefault(),
                            isBooked = _context.Booking.Any(b => b.SeatUuid == seat.Uuid && b.BillUu.ShowtimeUuid == request.Uuid)
                        }).ToList(),
                    }).FirstOrDefault();
                response.Data = ScreenSeatDTO;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }

        [HttpPost("get_list_combo_for_booking")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<PageListComboForBookingDTO>), description: "GetListComboForBooking Response")]
        public async Task<IActionResult> GetPageListCombo()
        {
            var response = new BaseResponseMessageItem<PageListComboForBookingDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                response.Data = _context.Combo.Where(x => x.Status != 0)
                    .Select(combo => new PageListComboForBookingDTO
                    {
                        Uuid = combo.Uuid,
                        ComboName = combo.ComboName,
                        ComboItems = combo.ComboItems,
                        ImageUrl = _context.Images.Where(x => x.OwnerUuid == combo.Uuid).Select(x => x.Path).FirstOrDefault(),
                        Price = combo.Price,
                        Status = combo.Status,

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

        [HttpPost("page_list_coupon_for_booking")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<CouponForBookingDTO>), description: "GetPageListCoupon Response")]
        public async Task<IActionResult> GetPageListCoupon()
        {
            var response = new BaseResponseMessageItem<CouponForBookingDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {

                response.Data = _context.Coupon.Where(x => x.Status == 1)
                    .Select(coupon => new CouponForBookingDTO()
                    {
                        Uuid = coupon.Uuid,
                        Code = coupon.Code,
                        Discount = coupon.Discount,
                        Status = coupon.Status,
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

        [HttpPost("create_bill_payment")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessage<PaymentResponseDTO>), description: "CreateBillPayment Response")]
        public async Task<IActionResult> CreateBillPayment(CreateBillRequest request)
        {
            var response = new BaseResponseMessage<PaymentResponseDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                // Tạo Bill
                var bill = new Bill()
                {
                    Uuid = Guid.NewGuid().ToString(),
                    UserUuid = validToken.UserUuid,
                    ShowtimeUuid = request.ShowtimeUuid,
                    CouponUuid = string.IsNullOrEmpty(request.CouponUuid) ? null : request.CouponUuid,
                    TotalPrice = request.TotalPrice,
                    PayPrice = request.PayPrice,
                    State = 0, // Chưa thanh toán
                    TimeCreated = DateTime.Now,
                    Status = 1,
                };
                Random random = new Random();
                string datePart = DateTime.Now.ToString("yyyyMMdd"); // Lấy ngày hiện tại với định dạng YYYYMMDD
                string randomDigits = random.Next(1000, 9999).ToString(); // 8 chữ số ngẫu nhiên
                bill.Code = datePart + "" + randomDigits;
                _context.Bill.Add(bill);

                // Tạo danh sách ghế
                foreach (var seat in request.Seats)
                {
                    var newSeat = new Booking()
                    {
                        BillUuid = bill.Uuid,
                        SeatUuid = seat.SeatUuid,
                        TicketUuid = seat.SeatPriceUuid,
                        Status = 1
                    };
                    _context.Booking.Add(newSeat);
                }

                // Tạo danh sách combo
                foreach (var combo in request.Combo)
                {
                    var newCombo = new BillCombo()
                    {
                        BillUuid = bill.Uuid,
                        ComboUuid = combo,
                        Status = 1
                    };
                    _context.BillCombo.Add(newCombo);
                }

                _context.SaveChanges();

                // Tạo URL thanh toán VNPay
                var vnpUrl = GenerateVnPayUrl(bill);

                // Trả về URL thanh toán cho Frontend
                response.Data = new PaymentResponseDTO { PaymentUrl = vnpUrl };
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

        private string GenerateVnPayUrl(Bill bill, string selectedBankCode = null)
        {
            var vnpUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            var tmnCode = "8LA1XGFW";
            var hashSecret = "TJXK8MZO7GTWP2UGNRSCJTVFLQURJREM";

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", tmnCode);
            vnpay.AddRequestData("vnp_Amount", ((int)(bill.PayPrice * 100)).ToString());
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_TxnRef", bill.Uuid);
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toán đơn hàng {bill.Code}");
            vnpay.AddRequestData("vnp_ReturnUrl", "http://localhost:5203/api/v1/Booking/vnpay-return");
            vnpay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));

            /*// Chỉ thêm mã ngân hàng nếu có
            vnpay.AddRequestData("vnp_BankCode", "NCB");*/

            var paymentUrl = vnpay.CreateRequestUrl(vnpUrl, hashSecret);
            return paymentUrl;
        }



        [HttpGet("vnpay-return")]
        public IActionResult VnPayReturn()
        {
            var vnpay = new VnPayLibrary();
            var hashSecret = "TJXK8MZO7GTWP2UGNRSCJTVFLQURJREM";

            // Lấy dữ liệu từ query string
            var requestQuery = HttpContext.Request.Query;
            foreach (var (key, value) in requestQuery)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value);
                }
            }

            // Xác thực chữ ký
            var isValidSignature = vnpay.ValidateSignature(requestQuery["vnp_SecureHash"], hashSecret);
            if (!isValidSignature)
            {
                return BadRequest("Chữ ký không hợp lệ.");
            }

            // Lấy dữ liệu từ VNPay
            var billUuid = vnpay.GetResponseData("vnp_TxnRef");
            var responseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var transactionId = vnpay.GetResponseData("vnp_TransactionNo");

            try
            {
                // Tìm Bill trong DB
                var bill = _context.Bill.FirstOrDefault(b => b.Uuid == billUuid);
                if (bill == null)
                {
                    return NotFound("Bill không tồn tại.");
                }

                // Kiểm tra trạng thái và cập nhật nếu cần
                if (bill.State == 0) // Chỉ cập nhật khi bill chưa được thanh toán
                {
                    bill.State = (sbyte)(responseCode == "00" ? 1 : 2); // Thành công: 1, Thất bại: 2
                    /*bill.TransactionId = transactionId;*/ // Ghi nhận mã giao dịch
                }

                // Lưu thông tin vào PaymentTransaction
                /*var paymentTransaction = new PaymentTransaction
                {
                    BillUuid = billUuid,
                    TransactionId = transactionId,
                    Amount = Convert.ToDecimal(vnpay.GetResponseData("vnp_Amount")) / 100, // Số tiền
                    ResponseCode = responseCode,
                    Message = vnpay.GetResponseData("vnp_Message"),
                    CreatedTime = DateTime.Now,
                    Status = responseCode == "00" ? 1 : -1 // Thành công: 1, Thất bại: -1
                };
                _context.PaymentTransaction.Add(paymentTransaction);*/

                _context.SaveChanges();

                // Chuyển hướng về FE
                /*return Redirect(responseCode == "00" ?
                    "http://localhost:3001/payment-success" :
                    "http://localhost:3001/payment-failed");*/

                return Redirect(responseCode == "00" ?
                    "http://localhost:3030/payment/success" :
                    "http://localhost:3030/payment/error");
            }
            catch (Exception ex)
            {
                // Log lỗi
                _logger.LogError(ex, "Lỗi khi xử lý callback từ VNPay.");
                return BadRequest("Có lỗi xảy ra khi xử lý giao dịch.");
            }
        }


    }
}
    
