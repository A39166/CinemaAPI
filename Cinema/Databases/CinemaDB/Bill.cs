using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Bill
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string UserUuid { get; set; } = null!;

    public string ShowtimeUuid { get; set; } = null!;

    public string? CouponUuid { get; set; }

    public int TotalPrice { get; set; }

    public sbyte? State { get; set; }

    public DateTime? TimeCreated { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<BillCombo> BillCombo { get; set; } = new List<BillCombo>();

    public virtual ICollection<Booking> Booking { get; set; } = new List<Booking>();

    public virtual Showtimes ShowtimeUu { get; set; } = null!;

    public virtual User UserUu { get; set; } = null!;
}
