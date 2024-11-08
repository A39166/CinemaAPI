using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Seat
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string ScreenUuid { get; set; } = null!;

    public string SeatCode { get; set; } = null!;

    public string SeatTypeUuid { get; set; } = null!;

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }

    public virtual Screen ScreenUu { get; set; } = null!;

    public virtual SeatType SeatTypeUu { get; set; } = null!;
}
