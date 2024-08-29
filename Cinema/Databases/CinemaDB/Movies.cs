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

    public int Rated { get; set; }

    /// <summary>
    /// 0-Chiếu sớm, 1-Chiếu thường
    /// </summary>
    public sbyte Format { get; set; }

    public float AverageReview { get; set; }

    public string? DirectorUuid { get; set; }

    public DateTime? RealeaseDate { get; set; }

    public sbyte Status { get; set; }

    public virtual Director? DirectorUu { get; set; }

    public virtual ICollection<MoviesCast> MoviesCast { get; set; } = new List<MoviesCast>();

    public virtual ICollection<MoviesGenre> MoviesGenre { get; set; } = new List<MoviesGenre>();

    public virtual ICollection<MoviesLanguage> MoviesLanguage { get; set; } = new List<MoviesLanguage>();

    public virtual ICollection<MoviesRegion> MoviesRegion { get; set; } = new List<MoviesRegion>();
}
