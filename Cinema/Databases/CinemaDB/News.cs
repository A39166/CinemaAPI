using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class News
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string ShortTitle { get; set; } = null!;

    public string? Content { get; set; }

    public int? View { get; set; }

    public DateTime TimeCreated { get; set; }

    /// <summary>
    /// 0-không sử dụng,1-Xuất bản,2-Nháp
    /// </summary>
    public sbyte Status { get; set; }
}
