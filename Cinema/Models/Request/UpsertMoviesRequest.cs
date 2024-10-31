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
        public sbyte Rated { get; set; }
        public float AverageReview { get; set; }
        public DateTime RealeaseDate { get; set; }
        [DefaultValue(2)]
        public sbyte Status {  get; set; }
        public string? ImagesUuid { get; set; }
        public string? DirectorUuid { get; set; }
        public List<string> Genre {  get; set; }
        public List<string> Cast { get; set; }
        public string? RegionUuid { get; set; }

    }
}
