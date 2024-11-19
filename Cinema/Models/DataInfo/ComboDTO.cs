using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class ComboDTO : BaseDTO
    {
        public string ComboName { get; set; } 

        public string ComboItems { get; set; } 

        public int? Price { get; set; }
        public string? ImageUrl { get; set; }

        public DateTime TimeCreated { get; set; }

        public sbyte Status { get; set; }
    }
}
