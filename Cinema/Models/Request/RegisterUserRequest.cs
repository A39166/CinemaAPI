using CinemaAPI.Models.BaseRequest;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models.Request
{
    public class RegisterUserRequest 
    {
       
        public string Email { get; set; }

        [Required(ErrorMessage = "Fullname field is required.")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Gender field is required.")]
        public sbyte Gender { get; set; }
        public DateOnly Birthday { get; set; }
        public string Password { get; set; }
        [DefaultValue(1)]
        public sbyte Role { get; set; } = 1;
        [DefaultValue(1)]
        public sbyte Status { get; set; } = 1;
    }
}
