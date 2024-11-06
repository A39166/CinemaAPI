using CinemaAPI.Models.BaseRequest;
using CinemaAPI.Models.DataInfo;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models.Request
{
    public class UpdateUserRequest : UuidRequest
    {

        [Required(ErrorMessage = "Fullname field is required.")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Gender field is required.")]
        public sbyte Gender { get; set; }
        public DateOnly Birthday { get; set; }
        public string? PhoneNumber { get; set; }
        [DefaultValue(1)]
        public sbyte Role { get; set; } 
        [DefaultValue(1)]
        public sbyte Status { get; set; }
    }
}
