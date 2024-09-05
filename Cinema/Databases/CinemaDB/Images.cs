using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Images
{
    public long Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string? OwnerUuid { get; set; }

    public string? OwnerType { get; set; }

    /// <summary>
    /// 1-Avatar, 2-poster, 3-Image
    /// </summary>
    public sbyte Type { get; set; }

    public string Path { get; set; } = null!;

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }
}
