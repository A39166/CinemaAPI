using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class Language
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    /// <summary>
    /// 1-Phụ đề,2-Lồng tiếng
    /// </summary>
    public sbyte LanguageType { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<MoviesLanguage> MoviesLanguage { get; set; } = new List<MoviesLanguage>();
}
