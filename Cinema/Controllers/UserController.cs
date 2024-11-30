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
using CinemaAPI.Utils;

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
                else
                {
                    var user = new User()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        Email = request.Email,
                        Fullname = request.Fullname,
                        Gender = request.Gender,
                        PhoneNumber = request.PhoneNumber,
                        Birthday = request.Birthday,
                        Password = request.Password,
                        Role =1,
                        Status = 1,
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
        [HttpPost("update_user")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "Update User Response")]
        public async Task<IActionResult> UpdateUser(UpdateUserRequest request)
        {
            var response = new BaseResponse();
            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
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
        [HttpPost("detail_user")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserDTO), description: "Detail User Response")]
        public async Task<IActionResult> DetailUser(UuidRequest request)
        {
            var response = new BaseResponseMessage<UserDTO>();
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

        [HttpPost("update_user_client")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateUserClient Response")]
        public async Task<IActionResult> UpdateUserClient(UpdateUserClientRequest request)
        {
            var response = new BaseResponse();
            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
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
                    _context.User.Update(user);
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
                                newimage.OwnerUuid = user.Uuid;
                                newimage.OwnerType = "user";
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
        [HttpPost("detail_user_client")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserClientDTO), description: "DetailUserClient Response")]
        public async Task<IActionResult> DetailUserClient()
        {
            var response = new BaseResponseMessage<UserClientDTO>();
            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var user = _context.User.Where(x => x.Uuid == validToken.UserUuid).FirstOrDefault(); ;
                if (user == null)
                {
                    throw new ErrorException(ErrorCode.USER_NOTFOUND);
                }
                else
                {
                    response.Data = new UserClientDTO()
                    {
                        Uuid = user.Uuid,
                        Email = user.Email,
                        Fullname = user.Fullname,
                        Gender = user.Gender,
                        Birthday = user.Birthday,
                        PhoneNumber = user.PhoneNumber,
                        TimeCreated = user.TimeCreated,
                        ImageUrl = _context.Images.Where(x => user.Uuid == x.OwnerUuid && x.Status == 1).Select(x => x.Path).FirstOrDefault(),
                        Status = user.Status,
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
        [HttpPost("change_password")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "ChangePassword Response")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var response = new BaseResponse();
            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var user = _context.User.Where(x => x.Uuid == validToken.UserUuid).FirstOrDefault(); ;
                if (user == null)
                {
                    throw new ErrorException(ErrorCode.USER_NOTFOUND);
                }
                
                else
                {
                    if (user.Password != request.OldPassword)
                    {
                        throw new ErrorException(ErrorCode.WRONG_OLD_PASS);
                    }
                    else
                    {
                        user.Password = request.NewPassword;
                        _context.User.Update(user);
                        _context.SaveChanges();
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

        [HttpPost("send_otp")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "SendOtpEmail Response")]
        public async Task<IActionResult> SendOtpEmail(SendOtpRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var account = _context.User.FirstOrDefault(x => x.Email == request.Email);
                if (account == null)
                {
                    response.error.SetErrorCode(ErrorCode.ACCOUNT_NOTFOUND, "Tài khoản không tồn tại. Vui lòng kiểm tra lại");
                    return BadRequest(response);
                }
                var otp = OtpService.GenerateAndStoreOtp(request.Email);
                _logger.LogInformation($"OTP for {request.Email}: {otp}");
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

        [HttpPost("enter_otp")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "EnterOtp Response")]
        public async Task<IActionResult> EnterOtp(EnterOtpReqDTO request)
        {
            var response = new BaseResponse();
            try
            {
                var isValid = OtpService.ValidateOtp(request.Email, request.Otp);
                var isExpiredOtp = OtpService.IsOtpExpired(request.Email, request.Otp);
                if (!isValid)
                {
                    throw new ErrorException(ErrorCode.OTP_INVALID);
                }
                if (isExpiredOtp)
                {
                    throw new ErrorException(ErrorCode.OTP_EXPIRED);
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
        [HttpPost("change_pass_forget")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "ChangePassForget Response")]
        public async Task<IActionResult> ChangePasswordForget(ChangePasswordForget request)
        {
            var response = new BaseResponse();

            try
            {
                var account = _context.User.FirstOrDefault(x => x.Email == request.Email);
                account.Password = request.NewPassword;
                _context.SaveChanges();
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

        public class OtpService
        {
            private static readonly Dictionary<string, (string Otp, DateTime Expiry)> OtpStorage = new Dictionary<string, (string, DateTime)>();
            private static readonly string FixedOtp = "123456"; // Mã OTP cố định
            private const int OtpExpiryMinutes = 1; // Thời gian hết hạn của mã OTP

            public static string GenerateAndStoreOtp(string email)
            {
                DateTime expiryTime = DateTime.Now.AddMinutes(OtpExpiryMinutes);
                OtpStorage[email] = (FixedOtp, expiryTime);
                return FixedOtp;
            }

            public static bool ValidateOtp(string email, string otp)
            {
                return otp == FixedOtp;
            }
            public static bool IsOtpExpired(string email, string otp)
            {
                // Lấy thời điểm tạo OTP
                if (OtpStorage.TryGetValue(email, out var otpData) && otpData.Otp == otp)
                {
                    DateTime otpCreationTime = otpData.Expiry;
                    // Tính thời gian đã trôi qua từ khi tạo OTP đến hiện tại
                    var timeElapsed = DateTime.Now;
                    // Kiểm tra xem thời gian đã trôi qua có vượt quá thời hạn của OTP không
                    return timeElapsed > otpCreationTime;
                }
                // Trả về true nếu không thể tìm thấy thời gian tạo OTP
                return true;
            }
        }

    }
}
