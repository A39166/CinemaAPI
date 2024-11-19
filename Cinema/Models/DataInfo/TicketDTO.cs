using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class TicketDTO : BaseDTO
    {
        public string SeatTypeUuid { get; set; }
        public string ScreenTypeUuid { get; set; }
        public sbyte DateState { get; set; }
        public int Price { get; set; }
        public sbyte Status { get; set; }
    }
}
