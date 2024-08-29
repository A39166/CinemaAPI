using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CinemaAPI.Models.BaseRequest;

namespace CinemaAPI.Models.Request
{
    public class UpsertDirectorRequest: UuidRequest
    {
        public string DirectorName { get; set; }
        public DateOnly? DirectorBirth { get; set; }
        public string? DirectorDescription {  get; set; }
        [DefaultValue(1)]
        public sbyte Status {  get; set; }

    }
}
