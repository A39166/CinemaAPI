﻿using CinemaAPI.Models.BaseRequest;
using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class CastDTO : DpsPagingParamBase
    {
        public string Uuid { get; set; }
        public string CastName { get; set; }
        public DateOnly? Birthday { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }
    }
}
