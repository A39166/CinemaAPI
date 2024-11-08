using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Movies
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string EngTitle { get; set; } = null!;

    public string? Trailer { get; set; }

    public string? Description { get; set; }

    public int Duration { get; set; }

    /// <summary>
    /// 1 - P, 2 - T13, 3- T16, 4- T18
    /// </summary>
    public sbyte Rated { get; set; }

    public float AverageReview { get; set; }

    public string DirectorUuid { get; set; } = null!;

    public DateOnly RealeaseDate { get; set; }

    public DateTime TimeCreated { get; set; }

    /// <summary>
    /// 0-Không sử dụng,1-Đang chiếu,2-Sắp chiếu,3-Chiếu sớm, 4-Không còn chiếu
    /// </summary>
    public sbyte Status { get; set; }

    public virtual Director DirectorUu { get; set; } = null!;

    public virtual ICollection<MoviesCast> MoviesCast { get; set; } = new List<MoviesCast>();

    public virtual ICollection<MoviesGenre> MoviesGenre { get; set; } = new List<MoviesGenre>();

    public virtual ICollection<MoviesRegion> MoviesRegion { get; set; } = new List<MoviesRegion>();

    public virtual ICollection<Showtimes> Showtimes { get; set; } = new List<Showtimes>();
}
