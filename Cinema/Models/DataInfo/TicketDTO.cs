using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class TicketDTO : BaseDTO
    {
        public string SeatTypeUuid { get; set; }
        public string ScreenTypeUuid { get; set; }
        public sbyte DateState { get; set; }
        public double Price { get; set; }
        public sbyte Status { get; set; }
    }
    public class PageListTicketDTO : BaseDTO
    {
        public sbyte SeatType { get; set; }
        public sbyte ScreenType { get; set; }
        public sbyte DateState { get; set; }
        public double Price { get; set; }
        public sbyte Status { get; set; }
    }
}
