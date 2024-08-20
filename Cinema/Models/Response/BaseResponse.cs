using Microsoft.AspNetCore.Mvc.Versioning;
using CinemaAPI.Enums;
using CinemaAPI.Extensions;
namespace CinemaAPI.Models.Response
{
    public class BaseResponse
    {
        public Error error { get; set; } = new Error();
        public class Error
        {
            public ErrorCode Code { get; set; }
            public string Message { get; set; }
            public Error(ErrorCode code = ErrorCode.SUCCESS)
            {
                Code = code;
                Message = code.ToDescriptionString();
            }

            public void SetErrorCode(ErrorCode code)
            {
                Code = code;
                Message = code.ToDescriptionString();
            }

            public void SetErrorCode(ErrorCode code, string? message)
            {
                Code = code;
                if(message != null) { Message = message; }
                else
                {
                    Message = code.ToDescriptionString();
                }
               
            }
        }
    }
}
