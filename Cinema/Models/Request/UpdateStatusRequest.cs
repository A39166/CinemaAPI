namespace CinemaAPI.Models.Request
{
    public class UpdateStatusRequest : UuidRequest
    {
        public sbyte Status { get; set; }
    }
}
