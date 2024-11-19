using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Coupon
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int Quantity { get; set; }

    public int? Used { get; set; }

    public int Discount { get; set; }

    public DateTime ExpiredDate { get; set; }

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<Bill> Bill { get; set; } = new List<Bill>();
}
