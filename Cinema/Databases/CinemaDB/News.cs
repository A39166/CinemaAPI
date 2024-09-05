using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class News
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }
}
