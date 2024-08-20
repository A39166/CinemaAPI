using CinemaAPI.AttributeExtend;
using CinemaAPI.Databases.CinemaDB;
using CinemaAPI.Enums;
using CinemaAPI.Models.Response;
using CinemaAPI.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace CinemaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("User Controller")]
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly DBContext _context;

        public UserController(DBContext context, ILogger<UserController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("register")]
        [DbpCert]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "Register Response")]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            var response = new BaseResponse();

            try
            {
                var email = _context.User.FirstOrDefault(d => d.Email == request.Email);
                if (email != null)
                {
                    // Bước 2.1: Khởi tạo đối tượng insert khi thêm mới
                    throw new Exception("Lỗi tài khoản đã có");
                }
                if (request.Password2 != request.Password) {
                    throw new Exception("Mật khẩu không khớp");
                }
                var user = new Databases.CinemaDB.User()
                {
                    Uuid = Guid.NewGuid().ToString(),
                    Email = request.Email,
                    Fullname = request.Fullname,
                    Gender = request.Gender,
                    Birthday = request.Birthday,
                    PhoneNumber = request.PhoneNumber,
                    Password = request.Password,
                    Role = request.Role,
                    Status = request.Status,

                };
            
                
                // Bước 2.2: Dùng lệnh add để thêm bản ghi vào CSDL
                _context.User.Add(user);
                _context.SaveChanges();
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
