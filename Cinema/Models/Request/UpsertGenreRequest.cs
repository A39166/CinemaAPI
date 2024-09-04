using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CinemaAPI.Models.BaseRequest;

namespace CinemaAPI.Models.Request
{
    public class UpsertGenreRequest: UuidRequest
    {
        public string GenreName { get; set; }
        public DateTime TimeCreated { get; set; }

    }
}
