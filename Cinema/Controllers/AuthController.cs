using CinemaAPI.AttributeExtend;
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
        [DbpCert]
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


                if (user != null)
                {
                    _token = new TokenInfo()
                    {
                        Token = Guid.NewGuid().ToString(),
                        UserName = user.Email,
                        UserUuid = user.Uuid
                    };

                    _token.ResetExpired();

                    // Lấy link ảnh avatar trong bảng Images nếu có
                    /*var avatarImage = _context.Images.Where(x => x.OwnerUuid == account.Uuid && x.Type == 1).FirstOrDefault();*/

                    response.Data = new LogInRespDTO()
                    {
                        Token = _token.Token,
                        Uuid = user.Uuid,
                        Email = user.Email,
                        Fullname = user.Fullname,
                        /*Avatar = avatarImage?.Path,*/
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
                    user.FcmToken = request.FCMToken;
                    _context.Sessions.Add(newSession);
                    _context.SaveChanges();
                }
                else
                {
                    response.error.SetErrorCode(ErrorCode.WRONG_LOGIN);
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
        [HttpPost("logout")]
        [DbpCert]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "LogOut Response")]
        public async Task<IActionResult> LogOut(DpsParamBase request)
        {
            var response = new BaseResponse();

            try
            {
                var _token = getTokenInfo(_context);

                if (_token != null)
                {
                    TokenManager.removeToken(_token.Token);

                    var oldSessions = _context.Sessions.Where(x => x.UserUuid == _token.UserUuid).ToList();

                    if (oldSessions != null && oldSessions.Count > 0)
                    {
                        foreach (var session in oldSessions)
                        {
                            session.Status = 1;
                        }
                    }

                    _context.SaveChanges();
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
