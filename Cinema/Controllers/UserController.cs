using CinemaAPI.Databases.CinemaDB;
using CinemaAPI.Enums;
using CinemaAPI.Models.Response;
using CinemaAPI.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using CinemaAPI.Extensions;
using CinemaAPI.Models.BaseRequest;
using CinemaAPI.Models.DataInfo;
using CinemaAPI.Configuaration;

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
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "Register Response")]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            var response = new BaseResponse();

            try
            {
                var email = _context.User.FirstOrDefault(d => d.Email == request.Email);
                if (email != null)
                {
                    response.error.SetErrorCode(ErrorCode.DUPLICATE_EMAIL);
                    return BadRequest(response);
                }
                else if (request.Password2 != request.Password) {
                    response.error.SetErrorCode(ErrorCode.MATCH_PASS);
                    return BadRequest(response);
                }
                else
                {
                    var user = new User()
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
                    _context.User.Add(user);
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
        [HttpPost("page_list_user")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<GetPageListUserDTO>), description: "GetPageListUser Response")]
        public async Task<IActionResult> GetPageListUser(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<GetPageListUserDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var query = _context.User;
                var lstUser = query.ToList();
                var totalcount = lstUser.Count();

                if (lstUser != null && lstUser.Count > 0)
                {
                    var result = lstUser.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<GetPageListUserDTO>();
                    }
                    foreach (var user in result)
                    {
                        var convertItemDTO = new GetPageListUserDTO()
                        {
                            Uuid = user.Uuid,
                            Email = user.Email,
                            Fullname = user.Fullname,
                            PhoneNumber = user.PhoneNumber,
                            ImageUrl = _context.Images.Where(x => user.Uuid == x.OwnerUuid).Select(x => x.Path).FirstOrDefault(),
                            Role = user.Role,
                            TimeCreated = user.TimeCreated,
                            Status = user.Status,
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
        [HttpPost("update_user")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "Update User Response")]
        public async Task<IActionResult> UpdateUser(UpdateUserRequest request)
        {
            var response = new BaseResponse();

            try
            {
                var user = _context.User.Where(x => x.Uuid == request.Uuid).FirstOrDefault(); ;
                if (user == null)
                {
                    throw new ErrorException(ErrorCode.USER_NOTFOUND);
                }
                else
                {

                    user.Fullname = request.Fullname;
                    user.Gender = request.Gender;
                    user.Birthday = request.Birthday;
                    user.PhoneNumber = request.PhoneNumber;
                    user.Role = request.Role;
                    user.Status = request.Status;
                    _context.User.Update(user);
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
        [HttpPost("detail_user")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserDTO), description: "Detail User Response")]
        public async Task<IActionResult> DetailUser(UuidRequest request)
        {
            var response = new BaseResponseMessage<UserDTO>();

            try
            {
                var user = _context.User.Where(x => x.Uuid == request.Uuid).SingleOrDefault(); ;
                if (user == null)
                {
                    throw new ErrorException(ErrorCode.USER_NOTFOUND);
                }
                else
                {
                    response.Data = new UserDTO()
                    {
                        Uuid = user.Uuid,
                        Email = user.Email,
                        Fullname = user.Fullname,
                        Gender = user.Gender,
                        Birthday = user.Birthday,
                        PhoneNumber = user.PhoneNumber,
                        Role = user.Role,
                        TimeCreated = user.TimeCreated,
                        ImageUrl = _context.Images.Where(x => user.Uuid == x.OwnerUuid && x.Status == 1).Select(x => x.Path).FirstOrDefault(),
                        Status = user.Status,
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
        [HttpPost("update_user_status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateUSerStatus Response")]
        public async Task<IActionResult> UpdateUserStatus(UpdateStatusRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var user = _context.User.Where(x => x.Uuid == request.Uuid).SingleOrDefault(); ;
                if (user == null)
                {
                    throw new ErrorException(ErrorCode.USER_NOTFOUND);
                }

                user.Status = request.Status;
                var img = _context.Images.Where(x => x.OwnerUuid == request.Uuid).SingleOrDefault();
                if (img != null)
                {
                    img.Status = request.Status;
                }
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
