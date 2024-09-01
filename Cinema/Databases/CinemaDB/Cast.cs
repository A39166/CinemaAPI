using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Cast
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string CastName { get; set; } = null!;

    public DateOnly? Birthday { get; set; }

    public string? Description { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<MoviesCast> MoviesCast { get; set; } = new List<MoviesCast>();
}
