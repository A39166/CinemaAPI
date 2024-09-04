namespace CinemaAPI.Models.DataInfo
{
    public class GenreDTO
    {
        public string Uuid { get; set; }
        public string GenreName { get; set; } 
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }
    }
}
