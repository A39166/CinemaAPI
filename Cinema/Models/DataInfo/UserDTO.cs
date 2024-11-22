using CinemaAPI.Databases.CinemaDB;
using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class UserDTO : BaseDTO
    {
        public string Fullname { get; set; } = null!;
        public sbyte Gender { get; set; }
        public DateOnly Birthday { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public sbyte Role { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }

    }
    public class UserClientDTO : BaseDTO
    {
        public string Fullname { get; set; } = null!;
        public sbyte Gender { get; set; }
        public DateOnly Birthday { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }

    }
}
