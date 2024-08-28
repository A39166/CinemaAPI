using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class MoviesLanguage
{
    public int Id { get; set; }

    public string MoviesUuid { get; set; } = null!;

    public string LanguageUuid { get; set; } = null!;

    public virtual Language LanguageUu { get; set; } = null!;

    public virtual Movies MoviesUu { get; set; } = null!;
}
