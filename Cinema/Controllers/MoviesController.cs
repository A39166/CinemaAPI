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
using System.Data;

namespace CinemaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Cast Controller")]
    public class MoviesController : BaseController
    {
        private readonly ILogger<MoviesController> _logger;
        private readonly DBContext _context;

        public MoviesController(DBContext context, ILogger<MoviesController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("upsert_movies")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpsertMovies Response")]
        public async Task<IActionResult> UpsertMovies(UpsertMoviesRequest request)
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

                    var movies = new Movies()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        Title = request.Title,
                        EngTitle = request.EngTitle,
                        Trailer = request.Trailer,
                        Description = request.Description,
                        Duration = request.Duration,
                        Rated = request.Rated,
                        AverageReview = request.AverageReview,
                        DirectorUuid = string.IsNullOrEmpty(request.DirectorUuid) ? null : request.DirectorUuid,
                        RealeaseDate = request.RealeaseDate,
                        Status = 1,
                    };
                    _context.Movies.Add(movies);
                    AddCast(movies.Uuid, request.Cast);
                    AddGenre(movies.Uuid, request.Genre);
                    AddRegion(movies.Uuid, request.RegionUuid);
                    if (!string.IsNullOrEmpty(request.ImagesUuid))
                    {
                        var image = _context.Images.FirstOrDefault(img => img.Uuid == request.ImagesUuid);
                        if (image != null)
                        {
                            image.OwnerUuid = movies.Uuid;
                            image.OwnerType = "movies";
                            image.Status = 1;
                            _context.Images.Update(image);
                        }
                    }
                    _context.SaveChanges();
                }
                else
                //cập nhập dữ liệu
                {
                    var movies = _context.Movies.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                    if (movies != null)
                    {
                        movies.Title = request.Title;
                        movies.EngTitle = request.EngTitle;
                        movies.Trailer = request.Trailer;
                        movies.Description = request.Description;
                        movies.Duration = request.Duration;
                        movies.Rated = request.Rated;
                        movies.AverageReview = request.AverageReview;
                        movies.DirectorUuid = string.IsNullOrEmpty(request.DirectorUuid) ? null : request.DirectorUuid;
                        movies.RealeaseDate = request.RealeaseDate;
                        _context.SaveChanges();
                        if (!string.IsNullOrEmpty(request.ImagesUuid))
                        {
                            var newimage = _context.Images.SingleOrDefault(img => img.Uuid == request.ImagesUuid);
                            if (newimage != null)
                            {
                                var oldImage = _context.Images.SingleOrDefault(img => img.OwnerUuid == movies.Uuid);
                                if (oldImage != null)
                                {
                                    _context.Images.Remove(oldImage);
                                }
                                newimage.OwnerUuid = movies.Uuid;
                                newimage.OwnerType = "movies";
                                newimage.Status = 1;
                                _context.Images.Update(newimage);
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
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }
        [HttpPost("page_list_movies")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<MoviesDTO>), description: "GetPageListMovies Response")]
        public async Task<IActionResult> GetPageListMovies(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<MoviesDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var query = _context.Movies;
                var lstMovies = query.ToList();
                var totalcount = query.Count();

                if (lstMovies != null && lstMovies.Count > 0)
                {
                    var result = lstMovies.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<MoviesDTO>();
                    }
                    foreach (var movies in result)
                    {
                        var convertItemDTO = new MoviesDTO()
                        {
                            Uuid = movies.Uuid,
                            Title = movies.Title,
                            EngTitle = movies.EngTitle,
                            Trailer = movies.Trailer,
                            Description = movies.Description,
                            Duration = movies.Duration,
                            Rated = movies.Rated,
                            AverageReview = movies.AverageReview,
                            DirectorUuid = movies.DirectorUuid,
                            RealeaseDate = movies.RealeaseDate,
                            Status = movies.Status,
                            ImageUrl = _context.Images.Where(x => movies.Uuid == x.OwnerUuid).Select(x => x.Path).FirstOrDefault(),
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
        [HttpPost("movies_detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(MoviesDTO), description: "GetMoviesDetail Response")]
        public async Task<IActionResult> GetMoviesDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<MoviesDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                //TODO: Write code late

                var movies = _context.Movies.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                if (movies != null)
                {
                    response.Data = new MoviesDTO()
                    {
                        Uuid = movies.Uuid,
                        Title = movies.Title,
                        EngTitle = movies.EngTitle,
                        Trailer = movies.Trailer,
                        Description = movies.Description,
                        Duration = movies.Duration,
                        Rated = movies.Rated,
                        AverageReview = movies.AverageReview,
                        DirectorUuid = movies.DirectorUuid,
                        RealeaseDate = movies.RealeaseDate,
                        Status = movies.Status,
                        ImageUrl = _context.Images.Where(x => movies.Uuid == x.OwnerUuid).Select(x => x.Path).FirstOrDefault(),
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
        /*[HttpPost("update_movies_status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateMoviesStatus Response")]
        public async Task<IActionResult> UpdateMoviesStatus(UpdateStatusRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var moviesstatus = _context.Movies.Where(x => x.Uuid == request.Uuid).SingleOrDefault();

                if (moviesstatus != null)
                {
                    moviesstatus.Status = request.Status;

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
        }*/
        private void AddCast(string movieUuid, List<string> CastUuid)
        {
            foreach (var cast in CastUuid)
            {
                var newMovieCast = new MoviesCast
                {
                    MoviesUuid = movieUuid,
                    CastUuid = cast,
                    TimeCreated = DateTime.Now,
                    Status = 1
                };
                _context.MoviesCast.Add(newMovieCast);
            }


        }
        private void AddGenre(string movieUuid, List<string> GenreUuid)
        {
            foreach (var genre in GenreUuid)
            {
                var newMovieGenre = new MoviesGenre
                {
                    MoviesUuid = movieUuid,
                    GenreUuid = genre,
                    TimeCreated = DateTime.Now,
                    Status = 1
                };
                _context.MoviesGenre.Add(newMovieGenre);
            }
        }
        private void AddRegion(string movieUuid, string RegionUuid)
        {
                var newMovieRegion = new MoviesRegion
                {
                    MoviesUuid = movieUuid,
                    RegionUuid = RegionUuid,
                    TimeCreated = DateTime.Now,
                    Status = 1
                };
                _context.MoviesRegion.Add(newMovieRegion);
        }
    }
    }
