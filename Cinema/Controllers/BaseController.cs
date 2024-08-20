using CinemaAPI.Databases.CinemaDB;
using CinemaAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Base Controller")]
    public class BaseController : Controller
    {
        protected TokenInfo getTokenInfo(DBContext _context)
        {
            if (HttpContext.Request.Headers.ContainsKey("Authorization") &&
            HttpContext.Request.Headers["Authorization"][0].StartsWith("Bearer "))
            {
                var token = HttpContext.Request.Headers["Authorization"][0]
                    .Substring("Bearer ".Length);

                var result = TokenManager.getTokenInfoByToken(token);

                if (result != null)
                    return result;

                //load token tu database
                var session = _context.Sessions.Include(x => x.UserUu).Where(x => x.Uuid == token && x.Status == 0).SingleOrDefault();

                if (session != null)
                {
                    var _token = new TokenInfo()
                    {
                        Token = session.Uuid,
                        UserName = session.UserUu.Email,
                        UserUuid = session.UserUuid
                    };

                    TokenManager.addToken(_token);

                    return _token;
                }
            }

            return null;
        }

        /*protected bool checkEmailOtp(DBContext _context, string email, string otp, string accActived, string note)
        {
            var result = false;
            var checkOtp = _context.OtpEmail.Where(x => x.Email.Equals(email) && x.Otp.Equals(otp) && x.Status == 0).FirstOrDefault();

            if (checkOtp != null)
            {
                checkOtp.Status = 1;
                checkOtp.AccountUuid = accActived;
                checkOtp.Notes = note;

                _context.SaveChanges();

                result = true;
            }

            //TODO: Remove late
            if (otp.Equals("123456"))
            {
                return true;
            }

            return result;
        }*/

        protected void logOutAllSession(DBContext _context, string userUuid)
        {
            //LogOut khoi session
            var session = _context.Sessions.Where(x => x.UserUuid == userUuid && x.Status == 0).ToList();

            if (session != null && session.Count > 0)
            {
                foreach (var item in session)
                {
                    if (item.Status == 0)
                    {
                        item.TimeLogout = DateTime.Now;
                        item.Status = 1;
                    }
                }
                _context.SaveChanges();
            }
        }

        protected TokenInfo validateToken(DBContext _context)
        {
            var token = getTokenInfo(_context);
            if (token != null)
            {
                if (token.IsExpired())
                {
                    logOutAllSession(_context, token.UserUuid);
                    return null;
                }
                else
                {
                    token.ResetExpired();

                    return token;
                }
            }

            return null;

        }
    }

}
