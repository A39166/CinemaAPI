using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CinemaAPI.Models.BaseRequest;

namespace CinemaAPI.Models.Request
{
    public class UpsertCouponRequest: UuidRequest
    {
        public string Code { get; set; }
        public int Quantity { get; set; }
        public int Discount {  get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [DefaultValue(1)]
        public sbyte Status {  get; set; }

    }
}
