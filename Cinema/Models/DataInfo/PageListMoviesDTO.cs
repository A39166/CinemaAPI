using CinemaAPI.Models.BaseRequest;
using System;
using System.Collections.Generic;

namespace CinemaAPI.Models.DataInfo
{
    public class PageListMoviesDTO : DpsPagingParamBase
    {
        public string Uuid { get; set; } 

        public string Title { get; set; } 
        public int Rated { get; set; }
        public int Duration { get; set; }
        public DateOnly RealeaseDate { get; set; }
        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// 0-Không còn chiếu,1-Đang chiếu,2-Sắp chiếu,3-Chiếu sớm
        /// </summary>
        public sbyte Status { get; set; }
    }
    public class PageListMoviesHomeDTO : DpsPagingParamBase
    {
        public string Uuid { get; set; }
        public string Title { get; set; }
        public List<ShortCategoryDTO> Genre { get; set; }
        public string ImageUrl { get; set; }
        public string Trailer { get; set; }
        public string Description { get; set; }
        public double AverageReview { get; set; }
        public int Rated { get; set; }
        /// <summary>
        /// 0-Không còn chiếu,1-Đang chiếu,2-Sắp chiếu,3-Chiếu sớm
        /// </summary>
        public sbyte Status { get; set; }
    }
    public class PageListMoviesClientDTO : DpsPagingParamBase
    {
        public string Uuid { get; set; }
        public string Title { get; set; }
        public List<ShortCategoryDTO> Genre { get; set; }
        public int Duration { get; set; }
        /*public double AverageReview { get; set; }*/
        public int Rated { get; set; }
        public string ImageUrl { get; set; }
        /// <summary>
        /// 0-Không còn chiếu,1-Đang chiếu,2-Sắp chiếu,3-Chiếu sớm
        /// </summary>
        public sbyte Status { get; set; }
    }
}

