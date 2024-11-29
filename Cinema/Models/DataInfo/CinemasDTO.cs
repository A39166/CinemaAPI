using System;
using System.Collections.Generic;

namespace CinemaAPI.Models.DataInfo
{
    public class CinemasDTO
    {
        public string Uuid { get; set; }

        public string CinemaName { get; set; }

        public string Address { get; set; }

        public string? Location { get; set; }

        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// 0 - đang khóa, 1 - hoạt động
        /// </summary>
        public sbyte Status { get; set; }
    }
    public class CinemasClientDTO
    {
        public string Uuid { get; set; }

        public string CinemaName { get; set; }
        public string Address { get; set; }

        public string? Location { get; set; }
        /// <summary>
        /// 0 - đang khóa, 1 - hoạt động
        /// </summary>
        public sbyte Status { get; set; }
    }

    public class ShortCinemaDTO
    {
        public string Uuid { get; set; }

        public string CinemaName { get; set; }
        public string Address { get; set; }

        /// <summary>
        /// 0 - đang khóa, 1 - hoạt động
        /// </summary>
        public sbyte Status { get; set; }
    }

}

