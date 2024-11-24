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
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<SeatForBookingDTO>), description: "GetListSeatForBooking Response")]
        public async Task<IActionResult> GetListSeat(UuidRequest request)
        {
            var response = new BaseResponseMessageItem<SeatForBookingDTO>();

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
                response.Data = _context.Seat.Where(x => x.ScreenUuid == st.ScreenUuid && x.Status == 1)
               .Select(seat => new SeatForBookingDTO
               {
                   Uuid = seat.Uuid,
                   SeatCode = seat.SeatCode,
                   SeatType = _context.SeatType
                    .Where(t => t.Uuid == seat.SeatTypeUuid)
                    .Select(t => t.Type)
                    .FirstOrDefault(),
                   Price = _context.Ticket.Where(p => p.ScreenTypeUuid == st.ScreenUu.ScreenTypeUu.Uuid &&
                                                 p.SeatTypeUuid == seat.SeatTypeUuid &&
                                                 p.DateState == dateState).Select(t => t.Price).FirstOrDefault(),
                   isBooked = _context.Booking
                    .Any(b => b.SeatUuid == seat.Uuid && b.BillUu.ShowtimeUuid == request.Uuid)


               }).ToList();
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
