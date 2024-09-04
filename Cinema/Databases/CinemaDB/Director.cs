using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Director
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string DirectorName { get; set; } = null!;

    public DateOnly? Birthday { get; set; }

    public string? Description { get; set; }

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<Movies> Movies { get; set; } = new List<Movies>();
}
