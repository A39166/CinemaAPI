using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Genre
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string GenreName { get; set; } = null!;

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<MoviesGenre> MoviesGenre { get; set; } = new List<MoviesGenre>();
}
