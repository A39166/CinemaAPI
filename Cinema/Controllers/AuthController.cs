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
using CinemaAPI.Configuaration;

namespace CinemaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Auth Controller")]
    public class AuthController : BaseController
    {
        private readonly ILogger<AuthController> _logger;
        private readonly DBContext _context;

        public AuthController(DBContext context, ILogger<AuthController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("login")]
        [SwaggerResponse(statusCode: 200, type: typeof(LogInRespDTO), description: "LogIn Response")]
        public async Task<IActionResult> LogIn(LogInRequest request)
        {
            var response = new BaseResponseMessage<LogInRespDTO>();
            request.Email = request.Email.Trim().ToLower();

            try
            {
                //Xóa token cũ đi
                var _token = TokenManager.getTokenInfoByUser(request.Email);

                if (_token != null)
                {
                    TokenManager.removeToken(_token.Token);
                }

                var user = _context.User.Where(x => x.Email == request.Email)
                                              .Where(x => x.Password == request.Password)
                                              .SingleOrDefault();
                if(user == null)
                {
                    throw new ErrorException(ErrorCode.WRONG_LOGIN);
                }
                if(user.Status == 2)
                {
                    throw new ErrorException(ErrorCode.LOCKED_ACCOUNT);
                }
                if (user != null)
                {
                    _token = new TokenInfo()
                    {
                        Token = Guid.NewGuid().ToString(),
                        UserName = user.Email,
                        UserUuid = user.Uuid
                    };

                    _token.ResetExpired();
                    response.Data = new LogInRespDTO()
                    {
                        Token = _token.Token,
                        Uuid = user.Uuid,
                        Email = user.Email,
                        Fullname = user.Fullname,
                        Gender = user.Gender,
                        Birthday = user.Birthday,
                        ImageUrl = _context.Images.Where(x => user.Uuid == x.OwnerUuid && x.Status == 1).Select(x => x.Path).FirstOrDefault(),
                        Role = user.Role,
                    };

                    TokenManager.addToken(_token);
                    TokenManager.clearToken();

                    var oldSessions = _context.Sessions.Where(x => x.UserUuid == user.Uuid).ToList();

                    if (oldSessions != null && oldSessions.Count > 0)
                    {
                        foreach (var session in oldSessions)
                        {
                            session.Status = 1;
                        }
                    }

                    var newSession = new Sessions()
                    {
                        Uuid = _token.Token,
                        UserUuid = _token.UserUuid,
                        TimeLogin = DateTime.Now,
                        Status = 0
                    };
                    _context.Sessions.Add(newSession);
                    _context.SaveChanges();
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
        [HttpPost("logout")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "LogOut Response")]
        public async Task<IActionResult> LogOut()
        {
            var response = new BaseResponse();

            try
            {
                var _token = getTokenInfo(_context);

                if (_token != null)
                {
                    TokenManager.removeToken(_token.Token);

                    var oldSessions = _context.Sessions.Where(x => x.Status == 0).Where(x => x.UserUuid == _token.UserUuid).ToList();

                    if (oldSessions != null && oldSessions.Count > 0)
                    {
                        foreach (var session in oldSessions)
                        {
                            session.Status = 1;
                            session.TimeLogout = DateTime.Now;
                            _context.Sessions.Update(session);
                        }
                        _context.SaveChanges();                        
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

        [HttpPost("check_token_timeout")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "CheckTokenTimeout Response")]
        public async Task<IActionResult> CheckTokenTimeout(CheckTokenRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var session = _context.Sessions.Where(x => x.Uuid == request.Token && x.Status == 0).FirstOrDefault();
                var tokenExpiration = session.TimeLogin.AddHours(1);
                if (session == null)
                {
                    response.error.SetErrorCode(ErrorCode.NOT_FOUND);
                    return Unauthorized(response);
                }
                if (DateTime.UtcNow > tokenExpiration)
                {
                    return Unauthorized(response);
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
