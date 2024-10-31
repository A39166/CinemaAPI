using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class MoviesCast
{
    public int Id { get; set; }

    public string MoviesUuid { get; set; } = null!;

    public string CastUuid { get; set; } = null!;

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }

    public virtual Cast CastUu { get; set; } = null!;

    public virtual Movies MoviesUu { get; set; } = null!;
}
