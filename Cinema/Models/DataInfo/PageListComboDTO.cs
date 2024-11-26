﻿using System;
using System.Collections.Generic;

namespace CinemaAPI.Models.DataInfo
{
    public class PageListComboDTO : BaseDTO
    {
        
        public string ComboName { get; set; } 
        public int? Price { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }
    }
    public class PageListComboForBookingDTO : BaseDTO
    {

        public string ComboName { get; set; }
        public string ComboItems { get; set; }
        public int? Price { get; set; }
        public sbyte Status { get; set; }
    }

}

