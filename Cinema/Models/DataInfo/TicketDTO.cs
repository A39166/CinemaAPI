using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class TicketDTO : BaseDTO
    {
        public sbyte SeatType { get; set; }
        public sbyte ScreenType { get; set; }
        public sbyte DateState { get; set; }
        public int Price { get; set; }
        public sbyte Status { get; set; }
    }
}
