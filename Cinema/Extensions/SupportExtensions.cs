using Newtonsoft.Json;
using CinemaAPI.Enums;
using CinemaAPI.Models.Response;
using CinemaAPI.Utils;

namespace CinemaAPI.Extensions
{
    public static class SupportExtension
    {
        public static T GetMessage<T>(this T resp, ErrorCode errorCode) where T : BaseResponse
        {

            resp.error = new(errorCode);

            return resp;
        }

        public static string ToJsonString<T>(this T data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public static string? EnumDescription<T>(int idx) where T : System.Enum
        {
            string resp = string.Empty;
            foreach (var value in System.Enum.GetValues(typeof(T)))
            {
                if ((int)value == idx)
                {
                    resp = value.ToDescriptionString();
                    break;
                }
            }

            return resp;
        }
        public static PagedList<T> TakePage<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
        {
            return PagedList<T>.ToPagedList(source, pageNumber, pageSize);
        }
    }
}
