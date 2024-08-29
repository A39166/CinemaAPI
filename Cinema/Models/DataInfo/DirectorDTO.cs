using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class DirectorDTO
    {
        public string Uuid { get; set; }
        public string DirectorName { get; set; }
        public DateOnly? DirectorBirth { get; set; }
        public string? DirectorDescription { get; set; }
        public sbyte Status { get; set; }
    }
}
