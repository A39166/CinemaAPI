namespace CinemaAPI.Models.Response
{
    public class LogInRespDTO
    {
        public string Token { get; set; }
        public string Uuid { get; set; } = null!;
        public string Email { get; set; } = null!;
        /*public string? Avatar { get; set; } = null!;*/
        public string Fullname { get; set; } = null!;
    }
}
