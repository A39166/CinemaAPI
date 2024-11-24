namespace CinemaAPI.Models.DataInfo
{
    public class ScreenForBookingDTO : BaseDTO
    {
        public int Row { get; set; }
        public int Collumn { get; set; }
        public SeatForBookingDTO ListSeat { get; set; }
    }
}
