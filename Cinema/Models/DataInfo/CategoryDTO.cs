using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class CategoryDTO : BaseDTO
    {
        public string Uuid { get; set; } = "";
        public string Name { get; set; } = null!;
        public sbyte Type {  get; set; }
    }
}
