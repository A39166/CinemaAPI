﻿using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class DirectorDTO
    {
        public string Uuid { get; set; }
        public string DirectorName { get; set; }
        public DateOnly? Birthday { get; set; }
        public string? Description { get; set; }
        public sbyte Status { get; set; }
    }
}