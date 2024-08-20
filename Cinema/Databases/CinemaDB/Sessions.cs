using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Sessions
{
    public long Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string UserUuid { get; set; } = null!;

    public DateTime TimeLogin { get; set; }

    public DateTime? TimeLogout { get; set; }

    /// <summary>
    /// 0: LogIn - 1: LogOut
    /// </summary>
    public sbyte Status { get; set; }

    public virtual User UserUu { get; set; } = null!;
}
