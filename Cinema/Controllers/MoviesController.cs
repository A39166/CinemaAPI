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
using CinemaAPI.Configuaration;

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
                    var existingMovie = _context.Movies.FirstOrDefault(m =>
                                        m.Title == request.Title &&
                                        m.RealeaseDate == request.RealeaseDate &&
                                        m.DirectorUuid == request.DirectorUuid && m.Status == 1);
                    if (existingMovie != null)
                    {
                        throw new ErrorException(ErrorCode.DUPLICATE_MOVIE);
                    }
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
                    var movies = _context.Movies.Where(x => x.Uuid == request.Uuid).FirstOrDefault();
                    if (movies == null) {
                        throw new ErrorException(ErrorCode.MOVIE_NOTFOUND);
                    }
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
                        if (!string.IsNullOrEmpty(request.ImagesUuid))
                        {
                            var oldImageUuid = _context.Images.Where(x => x.OwnerUuid == request.Uuid).Select(u => u.Path).FirstOrDefault();
                            if (oldImageUuid != request.ImagesUuid)
                            {
                                var newimage = _context.Images.FirstOrDefault(img => img.Uuid == request.ImagesUuid);
                                if (newimage != null)
                                {
                                    var oldImage = _context.Images.FirstOrDefault(img => img.OwnerUuid == request.Uuid);
                                    if (oldImage != null)
                                    {
                                        oldImage.Status = 0;
                                        _context.Images.Update(oldImage);
                                    }
                                    newimage.OwnerUuid = movies.Uuid;
                                    newimage.OwnerType = "movies";
                                    newimage.Status = 1;
                                    _context.Images.Update(newimage);
                                    _context.SaveChanges();
                                }

                            }
                        }
                        UpdateCast(movies.Uuid, request.Cast);
                        UpdateGenre(movies.Uuid, request.Genre);
                        UpdateRegion(movies.Uuid, request.RegionUuid);
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
        [HttpPost("page_list_movies")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<PageListMoviesDTO>), description: "GetPageListMovies Response")]
        public async Task<IActionResult> GetPageListMovies(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<PageListMoviesDTO>();

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
                        response.Data.Items = new List<PageListMoviesDTO>();
                    }
                    foreach (var movies in result)
                    {
                        var convertItemDTO = new PageListMoviesDTO()
                        {
                            Uuid = movies.Uuid,
                            Title = movies.Title,
                            Rated = movies.Rated,
                            RealeaseDate = movies.RealeaseDate,
                            Status = movies.Status,
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
                var movies = _context.Movies.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                if (movies == null)
                {
                    throw new ErrorException(ErrorCode.MOVIE_NOTFOUND);
                }
                else
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
                        Genre = _context.MoviesGenre.Where(mg => mg.MoviesUuid == movies.Uuid)
                        .Select(mg => new CategoryDTO
                        {
                            Uuid = mg.GenreUu.Uuid,
                            Name = mg.GenreUu.GenreName
                        })
                        .ToList(),
                        Cast = _context.MoviesCast.Where(mc => mc.MoviesUuid == movies.Uuid)
                        .Select(mc => new CategoryDTO
                        {
                            Uuid = mc.CastUu.Uuid,
                            Name = mc.CastUu.CastName
                        })
                        .ToList(),
                        Region = _context.MoviesRegion.Where(mr => mr.MoviesUuid == movies.Uuid)
                        .Select(mr => new CategoryDTO
                        {
                            Uuid = mr.RegionUu.Uuid,
                            Name = mr.RegionUu.RegionName
                        })
                        .ToList()
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
        [HttpPost("update_movies_status")]
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
                    var relatedCasts = _context.MoviesCast.Where(mc => mc.MoviesUuid == request.Uuid).ToList();
                    foreach (var cast in relatedCasts)
                    {
                        cast.Status = request.Status;
                    }

                    var relatedGenres = _context.MoviesGenre.Where(mg => mg.MoviesUuid == request.Uuid).ToList();
                    foreach (var genre in relatedGenres)
                    {
                        genre.Status = request.Status;
                    }

                    var relatedRegions = _context.MoviesRegion.Where(mr => mr.MoviesUuid == request.Uuid).ToList();
                    foreach (var region in relatedRegions)
                    {
                        region.Status = request.Status;
                    }
                    var img = _context.Images.Where(x => x.OwnerUuid == request.Uuid).SingleOrDefault();
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
        private void AddRegion(string movieUuid, List<string> RegionUuid)
        {
            foreach (var region in RegionUuid)
            {
                var newMovieRegion = new MoviesRegion
                {
                    MoviesUuid = movieUuid,
                    RegionUuid = region,
                    TimeCreated = DateTime.Now,
                    Status = 1
                };
                _context.MoviesRegion.Add(newMovieRegion);
            }
        }
        private void UpdateCast(string movieUuid, List<string> requestCastUuids)
        {
            var existingCasts = _context.MoviesCast
                                        .Where(mc => mc.MoviesUuid == movieUuid)
                                        .ToList();

            // Xóa các phần tử không có trong request
            foreach (var existingCast in existingCasts)
            {
                if (!requestCastUuids.Contains(existingCast.CastUuid))
                {
                    _context.MoviesCast.Remove(existingCast);
                }
            }

            // Thêm các phần tử mới từ request
            foreach (var castUuid in requestCastUuids)
            {
                if (!existingCasts.Any(ec => ec.CastUuid == castUuid))
                {
                    var newMovieCast = new MoviesCast
                    {
                        MoviesUuid = movieUuid,
                        CastUuid = castUuid,
                        TimeCreated = DateTime.Now,
                        Status = 1
                    };
                    _context.MoviesCast.Add(newMovieCast);
                }
            }
        }

        // Cập nhật Genre
        private void UpdateGenre(string movieUuid, List<string> requestGenreUuids)
        {
            var existingGenres = _context.MoviesGenre
                                         .Where(mg => mg.MoviesUuid == movieUuid)
                                         .ToList();

            // Xóa các phần tử không có trong request
            foreach (var existingGenre in existingGenres)
            {
                if (!requestGenreUuids.Contains(existingGenre.GenreUuid))
                {
                    _context.MoviesGenre.Remove(existingGenre);
                }
            }

            // Thêm các phần tử mới từ request
            foreach (var genreUuid in requestGenreUuids)
            {
                if (!existingGenres.Any(eg => eg.GenreUuid == genreUuid))
                {
                    var newMovieGenre = new MoviesGenre
                    {
                        MoviesUuid = movieUuid,
                        GenreUuid = genreUuid,
                        TimeCreated = DateTime.Now,
                        Status = 1
                    };
                    _context.MoviesGenre.Add(newMovieGenre);
                }
            }
        }

        // Cập nhật Region
        private void UpdateRegion(string movieUuid, List<string> requestRegionUuids)
        {
            var existingRegions = _context.MoviesRegion
                                          .Where(mr => mr.MoviesUuid == movieUuid)
                                          .ToList();

            // Xóa các phần tử không có trong request
            foreach (var existingRegion in existingRegions)
            {
                if (!requestRegionUuids.Contains(existingRegion.RegionUuid))
                {
                    _context.MoviesRegion.Remove(existingRegion);
                }
            }

            // Thêm các phần tử mới từ request
            foreach (var regionUuid in requestRegionUuids)
            {
                if (!existingRegions.Any(er => er.RegionUuid == regionUuid))
                {
                    var newMovieRegion = new MoviesRegion
                    {
                        MoviesUuid = movieUuid,
                        RegionUuid = regionUuid,
                        TimeCreated = DateTime.Now,
                        Status = 1
                    };
                    _context.MoviesRegion.Add(newMovieRegion);
                }
            }
        }
    }
    }
