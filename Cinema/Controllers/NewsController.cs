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
    [SwaggerTag("News Controller")]
    public class NewsController : BaseController
    {
        private readonly ILogger<NewsController> _logger;
        private readonly DBContext _context;

        public NewsController(DBContext context, ILogger<NewsController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("upsert_news")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpsertNews Response")]
        public async Task<IActionResult> UpsertNews(UpsertNewsRequest request)
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

                    var news = new News()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        Title = request.Title,
                        ShortTitle = request.ShortTitle,
                        Content = request.Content,
                        View = 0,
                        TimeCreated = DateTime.Now,
                        Status = request.Status,
                    };
                    _context.News.Add(news);
                    _context.SaveChanges();
                    if (!string.IsNullOrEmpty(request.ImagesUuid))
                    {
                        var image = _context.Images.FirstOrDefault(img => img.Uuid == request.ImagesUuid);
                        if (image != null)
                        {
                            image.OwnerUuid = news.Uuid;
                            image.OwnerType = "news";
                            image.Status = 1;
                            _context.Images.Update(image);
                            _context.SaveChanges();
                        }
                    }
                }
                else
                //cập nhập dữ liệu
                {
                    var news = _context.News.Where(x => x.Uuid == request.Uuid).FirstOrDefault();
                    if (news != null)
                    {
                        news.Title = request.Title;
                        news.ShortTitle = request.ShortTitle;
                        news.Content = request.Content;
                        news.TimeCreated = DateTime.Now;
                        news.Status = request.Status;
                        _context.News.Update(news);
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
                                    newimage.OwnerUuid = news.Uuid;
                                    newimage.OwnerType = "news";
                                    newimage.Status = 1;
                                    _context.Images.Update(newimage);
                                    _context.SaveChanges();
                                }

                            }
                        }
                        else
                        {
                            var oldImage = _context.Images.FirstOrDefault(img => img.OwnerUuid == request.Uuid && img.Status == 1);
                            if (oldImage != null)
                            {
                                oldImage.Status = 0;
                                _context.Images.Update(oldImage);
                                _context.SaveChanges();
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
        [HttpPost("page_list_news")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<PageListNewsDTO>), description: "GetPageListNews Response")]
        public async Task<IActionResult> GetPageListNews(DpsPagingParamBase request)
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
                        Content = newsdetail.Content,
                        View = newsdetail.View,
                        ImageUrl = _context.Images.Where(x => newsdetail.Uuid == x.OwnerUuid && x.Status == 1).Select(x => x.Path).FirstOrDefault(),
                        TimeCreated = newsdetail.TimeCreated,
                        Status = newsdetail.Status,
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
        [HttpPost("page_list_news_home")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<PageListNewsHomeDTO>), description: "GetPageListNewsHome Response")]
        public async Task<IActionResult> GetPageListNewsHome(/*DpsPagingParamBase request*/)
        {
            var response = new BaseResponseMessageItem<PageListNewsHomeDTO>();
            try
            {
                response.Data = _context.News.Where(x => x.Status == 1).OrderByDescending(news => news.TimeCreated)
                .Select(news => new PageListNewsHomeDTO
                {
                    Uuid = news.Uuid,
                    Title = news.Title,
                    ShortTitle = news.ShortTitle,
                    ImageUrl = _context.Images.Where(x => news.Uuid == x.OwnerUuid && x.Status == 1).Select(x => x.Path).FirstOrDefault(),
                    Status = news.Status,
                }).Take(4).ToList();
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
