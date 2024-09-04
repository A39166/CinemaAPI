using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class ScreenType
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    /// <summary>
    /// 1-2D,2-3D,3-IMAX
    /// </summary>
    public sbyte Type { get; set; }

    public virtual ICollection<Screen> Screen { get; set; } = new List<Screen>();
}
