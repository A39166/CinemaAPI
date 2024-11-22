using System;
using System.Collections.Generic;

namespace CinemaAPI.Models.DataInfo
{
    public class FormShowtimesByMoviesDTO : BaseDTO
    { 
        public DateOnly ShowDate {  get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public sbyte Status { get; set; }
    }
    public class PageListShowtimesByMoviesDTO
    {
        public string CinemaName { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public List<ScreenGroupDTO>? Screens { get; set; }
    }
    public class ScreenGroupDTO
    {
        public string ScreenTypeName {get; set; }
        public string LanguageType { get; set; }
        public List<FormShowtimesByMoviesDTO>? Showtimes { get; set; }
    }
}

