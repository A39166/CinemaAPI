using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CinemaAPI.Models.BaseRequest;

namespace CinemaAPI.Models.Request
{
    public class UpsertComboRequest: UuidRequest
    {
        public string ComboName { get; set; }
        public string ComboItems { get; set; }
        public int Price { get; set; }
        [DefaultValue(1)]
        public sbyte Status {  get; set; }
        public string? ImagesUuid { get; set; }

    }
}
