using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class CastDTO
    {
        public string Uuid { get; set; }
        public string CastName { get; set; }
        public DateOnly? Birthday { get; set; }
        public string? Description { get; set; }
        public sbyte Status { get; set; }
    }
}
