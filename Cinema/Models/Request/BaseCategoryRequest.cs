using CinemaAPI.Models.BaseRequest;
using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models.Request
{
    public class BaseCategoryRequest : BaseKeywordRequest
    {
       
        public string? Uuid { get; set; }
    }
}
