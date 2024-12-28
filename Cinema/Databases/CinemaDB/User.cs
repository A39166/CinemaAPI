﻿using System;
using System.Collections.Generic;

namespace CinemaAPI.Databases.CinemaDB;

public partial class User
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    /// <summary>
    /// 0-Nam , 1-Nữ , 2 - khác
    /// </summary>
    public sbyte Gender { get; set; }

    public DateOnly Birthday { get; set; }

    public string? PhoneNumber { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    /// <summary>
    /// 1-Admin, 2-Client
    /// </summary>
    public sbyte Role { get; set; }

    public DateTime TimeCreated { get; set; }

    /// <summary>
    /// 0 - không sử dụng, 1 - hoạt động, 2 - đang khóa
    /// </summary>
    public sbyte Status { get; set; }

    public virtual ICollection<Bill> Bill { get; set; } = new List<Bill>();

    public virtual ICollection<Sessions> Sessions { get; set; } = new List<Sessions>();
}
