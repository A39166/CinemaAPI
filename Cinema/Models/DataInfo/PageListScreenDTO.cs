using System;
using System.Collections.Generic;

namespace CinemaAPI.Models.DataInfo
{
    public class PageListScreenDTO : BaseDTO
    {
        public string ScreenName { get; set; } 
        public int Capacity { get; set; }
        public sbyte ScreenType {  get; set; }
        public string CinemaName { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }
    }

    public class PageListScreenForClientDTO : BaseDTO
    {
        public string ScreenName { get; set; }
        public sbyte Status { get; set; }
    }
}

