using CinemaAPI.Databases.CinemaDB;
using CinemaAPI.Models.BaseRequest;
using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class GetPageListUserDTO : DpsPagingParamBase
    {
        public string Uuid { get; set; }
        public string Fullname { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public sbyte Role { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }

}
}
