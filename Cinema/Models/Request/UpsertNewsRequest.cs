using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CinemaAPI.Models.BaseRequest;

namespace CinemaAPI.Models.Request
{
    public class UpsertNewsRequest: UuidRequest
    {
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string Content { get; set; }
        [DefaultValue(1)]
        public sbyte Status {  get; set; }
        public string? ImagesUuid { get; set; }

    }
}
