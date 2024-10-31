using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Showtimes
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string MoviesUuid { get; set; } = null!;

    public string ScreenUuid { get; set; } = null!;

    public DateOnly ShowDate { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public sbyte Status { get; set; }

    public virtual Movies MoviesUu { get; set; } = null!;

    public virtual Screen ScreenUu { get; set; } = null!;
}
