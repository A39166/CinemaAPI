using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Cinemas
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string CinemaName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Location { get; set; }

    public DateTime TimeCreated { get; set; }

    /// <summary>
    /// 0 - đang khóa, 1 - hoạt động
    /// </summary>
    public sbyte Status { get; set; }

    public virtual ICollection<Screen> Screen { get; set; } = new List<Screen>();
}
