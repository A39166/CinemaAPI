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

    public float AverageReview { get; set; }

    public int? DirectorUuid { get; set; }

    public DateTime? RealeaseDate { get; set; }

    public sbyte Status { get; set; }
}
