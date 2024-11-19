using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class ShowtimeDTO : BaseDTO
    {
        public string MoviesUuid { get; set; }
        public string CinemaUuid { get; set; }
        public string ScreenUuid { get; set; }
        public sbyte ScreenType { get; set; }
        public sbyte LanguageType { get; set; }
        public sbyte State { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public sbyte Status { get; set; }
    }
}
