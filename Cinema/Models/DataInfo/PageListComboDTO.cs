using System;
using System.Collections.Generic;

namespace CinemaAPI.Models.DataInfo
{
    public class PageListComboDTO : BaseDTO
    {
        
        public string ComboName { get; set; } 
        public double? Price { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }
    }
    public class PageListComboForBookingDTO : BaseDTO
    {

        public string ComboName { get; set; }
        public string ComboItems { get; set; }
        public string ImageUrl { get; set; }
        public double? Price { get; set; }
        public sbyte Status { get; set; }
    }
   public class ComboForBill : BaseDTO
    {
        public string ComboName { get; set; }
        public int Quantity {  get; set; }
        
    }

}

