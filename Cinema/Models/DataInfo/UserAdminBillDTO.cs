using CinemaAPI.Databases.CinemaDB;
using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class UserAdminBillDTO : BaseDTO
    {
        public string Fullname { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public sbyte Status { get; set; }

    }
}
