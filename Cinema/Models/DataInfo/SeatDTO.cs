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
        public double Price { get; set; }
        public bool isBooked { get; set; }
        public string TicketPriceUuid {  get; set; }
    }
    public class ScreenSeatForBookingDTO
    {
        public int Row { get; set; }
        public int Collumn { get; set; }
        public List<SeatForBookingDTO> Seats { get; set; }
    }
    public class SeatBillDTO : BaseDTO
    {
        public string SeatCode { get; set; }
    }

}
