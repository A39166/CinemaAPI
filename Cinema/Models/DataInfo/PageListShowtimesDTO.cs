using System;
using System.Collections.Generic;

namespace CinemaAPI.Models.DataInfo
{
    public class FormShowtimesDTO : BaseDTO
    {
        
        public string MoviesName { get; set; } 
        public sbyte ScreenType { get; set; }
        public sbyte LanguageType { get; set; }
        public DateOnly ShowDate {  get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public sbyte State { get; set; }
        public sbyte Status { get; set; }
    }
    public class PageListShowtimesDTO
    {
        public string CinemaName { get; set; }
        public List<FormSCreenDTO> Screens { get; set; }
    }
    public class FormSCreenDTO
    {
        public string ScreenName { get; set; }
        public List<FormShowtimesDTO>? Showtimes { get; set; }
    }
}

