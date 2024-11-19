using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class SeatType
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    /// <summary>
    /// 1-ghế thường,2-ghế vip,3-couple,4-Không khả dụng
    /// </summary>
    public sbyte Type { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Seat> Seat { get; set; } = new List<Seat>();

    public virtual ICollection<Ticket> Ticket { get; set; } = new List<Ticket>();
}
