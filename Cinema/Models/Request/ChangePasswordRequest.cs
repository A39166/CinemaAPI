using CinemaAPI.Models.BaseRequest;
using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models.Request
{
    public class ChangePasswordRequest 
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
