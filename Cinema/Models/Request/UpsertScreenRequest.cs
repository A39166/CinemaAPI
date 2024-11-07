using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CinemaAPI.Models.BaseRequest;

namespace CinemaAPI.Models.Request
{
    public class UpsertScreenRequest: UuidRequest
    {
        public string CinemaUuid { get; set; }
        public string ScreenName {  get; set; }
        public string ScreenTypeUuid { get; set; }
        public int Capacity { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }

    }
}
