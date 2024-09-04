using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Screen
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string? CinemaUuid { get; set; }

    public string ScreenName { get; set; } = null!;

    public string ScreenTypeUuid { get; set; } = null!;

    public int Capacity { get; set; }

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }

    public virtual Cinemas? CinemaUu { get; set; }

    public virtual ScreenType ScreenTypeUu { get; set; } = null!;
}
