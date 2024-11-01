using CinemaAPI.Models.BaseRequest;

namespace CinemaAPI.Models.Request
{
    public class BaseKeywordPageRequest:DpsPagingParamBase
    {
        public string? Keyword { get; set; }

        public sbyte? Status { get; set; }
    }
}
