﻿using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Booking
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string BillUuid { get; set; } = null!;

    public string SeatUuid { get; set; } = null!;

    public double? Price { get; set; }

    public sbyte Status { get; set; }

    public virtual Bill BillUu { get; set; } = null!;

    public virtual Seat SeatUu { get; set; } = null!;
}
