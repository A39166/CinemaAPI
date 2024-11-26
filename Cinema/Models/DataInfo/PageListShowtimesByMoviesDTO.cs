using System;
using System.Collections.Generic;

namespace CinemaAPI.Models.DataInfo
{
    public class PageListShowtimesByMoviesDTO
    {
        public string CinemaName { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public List<ScreenGroupDTO> Screens { get; set; }
    }
    public class ScreenGroupDTO
    {
        public string ScreenTypeName { get; set; }
        public List<FormShowtimesByMoviesDTO>? Showtimes { get; set; }
    }
    public class PageListShowtimesByCinemaDTO
    {
        public string MoviesUuid { get; set; }
        public string MoviesName { get; set; }
        public List<CategoryDTO> Genre { get; set; }
        public sbyte Rated { get; set; }
        public string ImageUrl { get; set; }
        public List<ScreenGroupDTO> Screens { get; set; }

    }
    public class FormShowtimesByMoviesDTO : BaseDTO
    {
        public DateOnly ShowDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public sbyte LanguageType { get; set; }
        public TimeOnly EndTime { get; set; }
        public sbyte Status { get; set; }
    }
}

