using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class ScreenDTO : BaseDTO
    {
        public string CinemaUuid { get; set; }

        public string ScreenName { get; set; }

        public sbyte ScreenType { get; set; }

        public int Capacity { get; set; }

        public int Row { get; set; }

        public int Collumn { get; set; }

        public DateTime TimeCreated { get; set; }

        public sbyte Status { get; set; }
    }
    public class ShortScreenDTO : BaseDTO
    {
        public string ScreenName  {get; set; }
        public string ScreenTypeName { get; set;}
    }
}
