using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CinemaAPI.Models.BaseRequest;

namespace CinemaAPI.Models.Request
{
    public class UpsertRegionRequest: UuidRequest
    {
       public string RegionName { get; set; }

    }
}
