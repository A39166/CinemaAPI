using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Region
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string RegionName { get; set; } = null!;

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<MoviesRegion> MoviesRegion { get; set; } = new List<MoviesRegion>();
}
