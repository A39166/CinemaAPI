using System.ComponentModel;

namespace CinemaAPI.Models.BaseRequest
{
    public class DpsPagingParamBase
    {
        [DefaultValue(20)]
        public int PageSize { get; set; } = 10;
        [DefaultValue(1)]
        public int Page { get; set; } = 1;
    }
}
