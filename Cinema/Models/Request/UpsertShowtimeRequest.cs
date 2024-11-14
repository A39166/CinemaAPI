using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CinemaAPI.Models.BaseRequest;

namespace CinemaAPI.Models.Request
{
    public class UpsertShowtimeRequest: UuidRequest
    {
        public string MoviesUuid { get; set; }
        public string ScreenUuid { get; set; }
        public sbyte LanguageType { get; set; }
        public DateOnly ShowDate {  get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        [DefaultValue(1)]
        public sbyte Status {  get; set; }

    }
}
