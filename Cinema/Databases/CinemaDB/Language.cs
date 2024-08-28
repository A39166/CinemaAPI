using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Language
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string LanguageName { get; set; } = null!;

    public sbyte Status { get; set; }

    public virtual ICollection<MoviesLanguage> MoviesLanguage { get; set; } = new List<MoviesLanguage>();
}
