using CinemaAPI.Models.BaseRequest;

namespace CinemaAPI.Models.DataInfo
{
    public class GenreDTO : DpsPagingParamBase
    {
        public string Uuid { get; set; }
        public string GenreName { get; set; } 
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }
    }
}
