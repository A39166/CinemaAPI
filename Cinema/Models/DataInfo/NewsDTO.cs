﻿using CinemaAPI.Models.BaseRequest;
using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class NewsDTO : BaseDTO
    {
        public string Title { get; set; }
        public int? View { get; set; }
        public string? ImageUrl { get; set; }
        public string ShortTitle { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }
    }
}
