using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Combo
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string ComboName { get; set; } = null!;

    public string ComboItems { get; set; } = null!;

    public int Price { get; set; }

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<BillCombo> BillCombo { get; set; } = new List<BillCombo>();
}
