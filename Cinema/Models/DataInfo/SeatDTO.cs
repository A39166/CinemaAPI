using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class ShortSeatDTO : BaseDTO
    {
        public string SeatCode { get; set; }

        public sbyte SeatType { get; set; }
    }
}
