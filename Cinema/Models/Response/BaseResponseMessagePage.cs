namespace CinemaAPI.Models.Response
{
    public class BaseResponseMessagePage<DTO> : BaseResponse
    {
        public DataPageResponse<DTO> Data { get; set; } = new DataPageResponse<DTO>();
    }
}
