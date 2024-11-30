using CinemaAPI.Models.BaseRequest;

namespace CinemaAPI.Models.Request
{
    public class ChangePasswordForget 
    {
        public string Otp {  get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }

    }
}
