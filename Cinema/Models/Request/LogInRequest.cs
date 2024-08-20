using System.ComponentModel.DataAnnotations;
using CinemaAPI.Models.BaseRequest;

namespace CinemaAPI.Models.Request
{
    public class LogInRequest : DpsParamBase
    {
       
        public string Email { get; set; }
        public string Password { get; set; }

        public string? FCMToken { get; set; }
    }
}
