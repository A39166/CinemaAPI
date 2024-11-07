using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class SeatType
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string SeatTypeName { get; set; } = null!;
}
