using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Ticket
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string ScreenTypeUuid { get; set; } = null!;

    public string SeatTypeUuid { get; set; } = null!;

    /// <summary>
    /// 1-Trong tuần,2-Cuối tuần
    /// </summary>
    public sbyte DateState { get; set; }

    public int Price { get; set; }

    public sbyte Status { get; set; }

    public virtual ScreenType ScreenTypeUu { get; set; } = null!;

    public virtual SeatType SeatTypeUu { get; set; } = null!;
}
