namespace CinemaAPI.Models.Request
{
    public class CheckTokenRequest : UuidRequest
    {
        public string Token { get; set; }
    }
}
