using System.ComponentModel;

namespace CinemaAPI.Models.Request
{
    public class UpsertCinemaRequest: UuidRequest
    {
        public string CinemaName { get; set; } 

        public string Address { get; set; } 

        public string? Location { get; set; }

        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// 0 - đang khóa, 1 - hoạt động
        /// </summary>
        public sbyte Status { get; set; }

    }
}
