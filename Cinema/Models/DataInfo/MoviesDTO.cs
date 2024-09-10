using System;
using System.Collections.Generic;

namespace CinemaAPI.Models.DataInfo
{
    public class MoviesDTO
    {

        public string Uuid { get; set; } 

        public string Title { get; set; } 

        public string EngTitle { get; set; }

        public string? Trailer { get; set; }

        public string? Description { get; set; }

        public int Duration { get; set; }

        public int Rated { get; set; }

        public float AverageReview { get; set; }

        public string? DirectorUuid { get; set; }

        public DateTime RealeaseDate { get; set; }

        /// <summary>
        /// 0-Không còn chiếu,1-Đang chiếu,2-Sắp chiếu,3-Chiếu sớm
        /// </summary>
        public sbyte Status { get; set; }
        public string? ImageUrl { get; set; }
    }
}

