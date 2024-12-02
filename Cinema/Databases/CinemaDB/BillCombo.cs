using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class BillCombo
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string BillUuid { get; set; } = null!;

    public string ComboUuid { get; set; } = null!;

    public sbyte Status { get; set; }

    public int Quantity { get; set; }

    public virtual Bill BillUu { get; set; } = null!;

    public virtual Combo ComboUu { get; set; } = null!;
}
