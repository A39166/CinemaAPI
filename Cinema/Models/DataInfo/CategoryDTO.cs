using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class CategoryDTO : BaseDTO
    {
        public string Name { get; set; } = null!;
        public sbyte Type {  get; set; }
    }
    public class ShortCategoryDTO : BaseDTO
    {
        public string Name { get; set; } = null!;
    }
}
