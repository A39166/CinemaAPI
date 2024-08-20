using CinemaAPI.Models.BaseRequest;
using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models.Request
{
    public class UuidRequest : DpsParamBase
    {
       
        public string? Uuid { get; set; }
    }
}
