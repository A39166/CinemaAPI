namespace CinemaAPI.Models.Response
{
    public class DataPageResponse<DTO>
    {
        public List<DTO> Items { get; set; } = new List<DTO>();
        public Paginations Pagination { get; set; } = new();
    }
}
