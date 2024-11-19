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
using CinemaAPI.Configuaration;

namespace CinemaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Ticket Controller")]
    public class TicketController : BaseController
    {
        private readonly ILogger<TicketController> _logger;
        private readonly DBContext _context;

        public TicketController(DBContext context, ILogger<TicketController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("upsert_ticket_price")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpsertTicketPrice Response")]
        public async Task<IActionResult> UpsertTicketPrice(UpsertTicketPriceRequest request)
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

                    var ticket = new Ticket()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        ScreenTypeUuid = request.ScreenTypeUuid,
                        SeatTypeUuid = request.SeatTypeUuid,
                        DateState = request.DateState,
                        Price = request.Price,
                        Status = 1,
                    };
                    _context.Ticket.Add(ticket);
                    _context.SaveChanges();
                }
                else
                //cập nhập dữ liệu
                {
                    var ticket = _context.Ticket.Where(x => x.Uuid == request.Uuid && x.Status != 0).FirstOrDefault();
                    if(ticket == null)
                    {
                        throw new ErrorException(ErrorCode.NOT_FOUND);
                    }
                    if (ticket != null)
                    {
                        ticket.ScreenTypeUuid = request.ScreenTypeUuid;
                        ticket.SeatTypeUuid = request.SeatTypeUuid;
                        ticket.DateState = request.DateState;
                        ticket.Price = request.Price;
                        _context.Ticket.Update(ticket);
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
        [HttpPost("page_list_ticket_price")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<PageListTicketDTO>), description: "GetPageListTicket Response")]
        public async Task<IActionResult> GetPageListTicket(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<PageListTicketDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var lstTicket = _context.Ticket.Where(x => x.Status == 1).ToList();
                var totalcount = lstTicket.Count();

                if (lstTicket != null && lstTicket.Count > 0)
                {
                    var result = lstTicket.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<PageListTicketDTO>();
                    }
                    foreach (var ticket in result)
                    {
                        var convertItemDTO = new PageListTicketDTO()
                        {
                            Uuid = ticket.Uuid,
                            SeatType = _context.SeatType.Where(x => x.Uuid == ticket.SeatTypeUuid).Select(x => x.Type).FirstOrDefault(),
                            ScreenType = _context.ScreenType.Where(x => x.Uuid == ticket.ScreenTypeUuid).Select(x => x.Type).FirstOrDefault(),
                            DateState = ticket.DateState,
                            Price = ticket.Price,
                            Status = ticket.Status,
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
        [HttpPost("ticket_detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(TicketDTO), description: "GetTicketPriceDetail Response")]
        public async Task<IActionResult> GetTicketPriceDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<TicketDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var ticket = _context.Ticket.Where(x => x.Uuid == request.Uuid && x.Status != 0).SingleOrDefault();
                if (ticket != null)
                {
                    response.Data = new TicketDTO()
                    {
                        Uuid = ticket.Uuid,
                        SeatTypeUuid = ticket.SeatTypeUuid,
                        ScreenTypeUuid = ticket.ScreenTypeUuid,
                        DateState = ticket.DateState,
                        Price = ticket.Price,
                        Status = ticket.Status,
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
        [HttpPost("update_ticket_status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateTicketStatus Response")]
        public async Task<IActionResult> UpdateTicketStatus(UpdateStatusRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var ticket = _context.Ticket.Where(x => x.Uuid == request.Uuid && x.Status != 0).SingleOrDefault();

                if (ticket != null)
                {
                    ticket.Status = request.Status;

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
