using CinemaAPI.Models.BaseRequest;
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
    }
    public class PageListShowtimesByMoviesRequest
    {

        public string? MoviesUuid { get; set; }
        public DateOnly? FindDate { get; set; }
    }
}
