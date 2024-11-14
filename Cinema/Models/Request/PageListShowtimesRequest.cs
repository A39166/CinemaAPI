﻿using CinemaAPI.Models.BaseRequest;
using CinemaAPI.Models.DataInfo;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models.Request
{
    public class PageListShowtimesRequest : DpsPagingParamBase
    {

        public string? CinemaUuid { get; set; }
        public string? ScreenUuid { get; set; }
        public DateOnly? FindDate {  get; set; }
        [DefaultValue(1)]
        public sbyte? Status { get; set; }
    }
}
