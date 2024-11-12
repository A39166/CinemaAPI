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

namespace CinemaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Combo Controller")]
    public class ComboController : BaseController
    {
        private readonly ILogger<ComboController> _logger;
        private readonly DBContext _context;

        public ComboController(DBContext context, ILogger<ComboController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("upsert_combo")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpsertCombo Response")]
        public async Task<IActionResult> UpsertCombo(UpsertComboRequest request)
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

                    var combo = new Combo()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        ComboName = request.ComboName,
                        ComboItems = request.ComboItems,
                        Price = request.Price,
                        TimeCreated = DateTime.Now,
                        Status = 1,
                    };
                    _context.Combo.Add(combo);
                    _context.SaveChanges();
                    if (!string.IsNullOrEmpty(request.ImagesUuid))
                    {
                        var image = _context.Images.FirstOrDefault(img => img.Uuid == request.ImagesUuid);
                        if (image != null)
                        {
                            image.OwnerUuid = combo.Uuid;
                            image.OwnerType = "combo";
                            image.Status = 1;
                            _context.Images.Update(image);
                            _context.SaveChanges();
                        }
                    }
                }
                else
                //cập nhập dữ liệu
                {
                    var combo = _context.Combo.Where(x => x.Uuid == request.Uuid).FirstOrDefault();
                    if (combo != null)
                    {
                        combo.ComboName = request.ComboName;
                        combo.ComboItems = request.ComboItems;
                        combo.Price = request.Price;
                        combo.TimeCreated = DateTime.Now;
                        combo.Status = request.Status;
                        _context.Combo.Update(combo);
                        _context.SaveChanges();
                        if (!string.IsNullOrEmpty(request.ImagesUuid))
                        {
                            var oldImageUuid = _context.Images.Where(x => x.OwnerUuid == request.Uuid).Select(u => u.Path).FirstOrDefault();
                            if (oldImageUuid != request.ImagesUuid)
                            {
                                var newimage = _context.Images.FirstOrDefault(img => img.Uuid == request.ImagesUuid);
                                if (newimage != null)
                                {
                                    var oldImage = _context.Images.FirstOrDefault(img => img.OwnerUuid == request.Uuid && img.Status == 1);
                                    if (oldImage != null)
                                    {
                                        oldImage.Status = 0;
                                        _context.Images.Update(oldImage);
                                    }
                                    newimage.OwnerUuid = combo.Uuid;
                                    newimage.OwnerType = "combo";
                                    newimage.Status = 1;
                                    _context.Images.Update(newimage);
                                    _context.SaveChanges();
                                }

                            }
                        }
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
        [HttpPost("page_list_combo")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<PageListNewsDTO>), description: "GetPageListCombo Response")]
        public async Task<IActionResult> GetPageListCombo(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<PageListNewsDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var lstNews = _context.News.Where(x => x.Status != 0)
                                           .ToList();

                var totalcount = lstNews.Count();

                if (lstNews != null && lstNews.Count > 0)
                {
                    var result = lstNews.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<PageListNewsDTO>();
                    }
                    foreach (var news in result)
                    {
                        var convertItemDTO = new PageListNewsDTO()
                        {
                            Uuid = news.Uuid,
                            Title = news.Title,
                            View = news.View,
                            TimeCreated = news.TimeCreated,
                            Status = news.Status,
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
        [HttpPost("news_detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(NewsDTO), description: "GetNewsDetail Response")]
        public async Task<IActionResult> GetNewsDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<NewsDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                //TODO: Write code late

                var newsdetail = _context.News.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                if (newsdetail != null)
                {
                    response.Data = new NewsDTO()
                    {
                        Uuid = newsdetail.Uuid,
                        Title = newsdetail.Title,
                        ShortTitle = newsdetail.ShortTitle,
                        View = newsdetail.View,
                        ImageUrl = _context.Images.Where(x => newsdetail.Uuid == x.OwnerUuid && x.Status == 1).Select(x => x.Path).FirstOrDefault(),
                        TimeCreated = newsdetail.TimeCreated,
                        Status = newsdetail.Status,
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
        [HttpPost("update_news_status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateNewsStatus Response")]
        public async Task<IActionResult> UpdateNewsStatus(UpdateStatusRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var newsstatus = _context.News.Where(x => x.Uuid == request.Uuid).SingleOrDefault();

                if (newsstatus != null)
                {
                    newsstatus.Status = request.Status;
                    var img = _context.Images.Where(x => x.OwnerUuid == request.Uuid && x.Status == 1).SingleOrDefault();
                    if (img != null)
                    {
                        img.Status = request.Status;
                    }
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
