using System.ComponentModel;

namespace CinemaAPI.Models.Request
{
    public class UpsertMoviesRequest: UuidRequest
    {
        public string Title { get; set; }
        public string EngTitle { get; set; }
        public string Trailer { get; set; }
        public string? Description {  get; set; }
        public int Duration { get; set; }
        public int Rated { get; set; }
        public float AverageReview { get; set; }
        public DateOnly? RealeaseDate { get; set; }
        [DefaultValue(2)]
        public sbyte Status {  get; set; }
        public string? ImagesUuid { get; set; }
        public string? DirectorUuid { get; set; }

    }
}
