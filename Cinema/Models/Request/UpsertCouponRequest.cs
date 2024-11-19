using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CinemaAPI.Models.BaseRequest;

namespace CinemaAPI.Models.Request
{
    public class UpsertCouponRequest: UuidRequest
    {
        public string CastName { get; set; }
        public DateOnly? Birthday { get; set; }
        public string? Description {  get; set; }
        public DateTime TimeCreated { get; set; }
        [DefaultValue(1)]
        public sbyte Status {  get; set; }
        public string? ImagesUuid { get; set; }

    }
}
