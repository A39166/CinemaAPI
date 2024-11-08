using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CinemaAPI.Models.BaseRequest;

namespace CinemaAPI.Models.Request
{
    public class CreateScreenRequest
    {
        public string CinemaUuid { get; set; }
        public string ScreenName {  get; set; }
        public sbyte ScreenType { get; set; }
        public int Capacity { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<SeatForm>? Seats {  get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }

    }
    public class SeatForm
    {
        public string SeatUuid { get; set; }
        public string SeatName { get; set; }
        public sbyte SeatType { get; set; }
    }
}
