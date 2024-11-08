using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class SeatType
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    /// <summary>
    /// 1-ghế thường,2-ghế vip,3-couple
    /// </summary>
    public sbyte Type { get; set; }

    public virtual ICollection<Seat> Seat { get; set; } = new List<Seat>();
}
