using CinemaAPI.Models.BaseRequest;

namespace CinemaAPI.Models.Request
{
    public class EnterOtpReqDTO 
    {
        public string Otp { get; set; }
        public string Email { get; set; }
    }
}
