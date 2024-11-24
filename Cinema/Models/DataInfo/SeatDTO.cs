using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class ShortSeatDTO : BaseDTO
    {
        public string SeatCode { get; set; }

        public sbyte SeatType { get; set; }
    }
    public class SeatForBookingDTO : BaseDTO
    {
        public string SeatCode { get; set; }

        public sbyte SeatType { get; set; }
        public int Price { get; set; }
        public bool isBooked { get; set; } 
    }

}
