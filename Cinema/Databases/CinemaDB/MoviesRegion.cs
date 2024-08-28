using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class MoviesRegion
{
    public int Id { get; set; }

    public string MoviesUuid { get; set; } = null!;

    public string RegionUuid { get; set; } = null!;

    public virtual Movies MoviesUu { get; set; } = null!;

    public virtual Region RegionUu { get; set; } = null!;
}
