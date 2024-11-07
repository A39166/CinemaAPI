using CinemaAPI.Models.BaseRequest;
using System;
using System.Collections.Generic;

namespace CinemaAPI.Models.DataInfo
{
    public class PageListScreenDTO : BaseDTO
    {
        public string ScreenName { get; set; } 
        public int Capacity { get; set; }
        public sbyte ScreenType {  get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }
    }
}

