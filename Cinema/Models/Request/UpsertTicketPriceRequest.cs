using System.ComponentModel;

namespace CinemaAPI.Models.Request
{
    public class UpsertTicketPriceRequest: UuidRequest
    {
        public string ScreenTypeUuid { get; set; }

        public string SeatTypeUuid { get; set; }
        public sbyte DateState { get; set; }
        public int Price { get; set; }

        /// <summary>
        /// 0 - đang khóa, 1 - hoạt động
        /// </summary>
        public sbyte Status { get; set; }

    }
}
