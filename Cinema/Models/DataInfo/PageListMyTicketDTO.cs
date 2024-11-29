using CinemaAPI.Models.BaseRequest;
using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class PageListMyTicketDTO : BaseDTO
    {
        public string Code { get; set; }
        public MyTicketMoviesDTO Movie { get; set; }
        public string ScreenName { get; set; }
        public string CinemaName { get; set; }
        public sbyte State { get; set; }
        public double PayPrice { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public sbyte Status { get; set; }
    }

}
