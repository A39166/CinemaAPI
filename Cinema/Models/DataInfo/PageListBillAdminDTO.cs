using CinemaAPI.Models.BaseRequest;
using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class PageListBillAdminDTO : BaseDTO
    {
        public string Code { get; set; }
        public string MovieName { get; set; }
        public string ScreenName { get; set; }
        public string CinemaName { get; set; }
        public sbyte State { get; set; }
        public double PayPrice { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }
    }
    public class PageListBillClientDTO : BaseDTO
    {
        public string Code { get; set; }
        public string MovieName { get; set; }
        public sbyte State { get; set; }
        public double PayPrice { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }
    }

}
