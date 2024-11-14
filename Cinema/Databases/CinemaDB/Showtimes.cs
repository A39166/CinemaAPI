using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Showtimes
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string MoviesUuid { get; set; } = null!;

    public string ScreenUuid { get; set; } = null!;

    /// <summary>
    /// 1-Phụ đề,2-Lồng tiếng
    /// </summary>
    public sbyte LanguageType { get; set; }

    /// <summary>
    /// 0-Sắp chiếu,1-Đang chiếu,2-Đã chiếu
    /// </summary>
    public sbyte State { get; set; }

    public DateOnly ShowDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<Bill> Bill { get; set; } = new List<Bill>();

    public virtual Movies MoviesUu { get; set; } = null!;

    public virtual Screen ScreenUu { get; set; } = null!;
}
