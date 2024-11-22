using System;
using System.Collections.Generic;

namespace CinemaAPI.Models.DataInfo
{
    public class PageListNewsDTO : BaseDTO
    {
        
        public string Title { get; set; } 
        public int? View { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }
    }

    public class PageListNewsHomeDTO : BaseDTO
    {
        public string? ImageUrl { get; set; }
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public sbyte Status { get; set; }
    }
}

