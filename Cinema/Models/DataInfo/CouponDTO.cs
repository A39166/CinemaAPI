using CinemaAPI.Models.BaseRequest;
using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class CouponDTO : BaseDTO
    {
        public string Code { get; set; }
        public int Quantity { get; set; }
        public int? Used { get; set; }
        public int Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [DefaultValue(1)]
        public sbyte Status { get; set; }
    }
    public class CouponForBookingDTO : BaseDTO
    {
        public string Code { get; set; }
        public int Discount { get; set; }
        [DefaultValue(1)]
        public sbyte Status { get; set; }
    }
}
