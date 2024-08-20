using Newtonsoft.Json.Linq;
using System.Net;
using CinemaAPI.Configuaration;

namespace CinemaAPI.Utils
{
    public class HttpService
    {
        private static readonly string EMAIL_SEND_NORMAL = "/SendMail";
        public static bool sendEmail(ILogger _logger, string toEmail, string title, string content)
        {
            try
            {
                var client = new HttpClient();

                var body = new JObject
                {
                    ["toEmail"] = toEmail,
                    ["title"] = title,
                    ["content"] = content
                };

                var _content = new StringContent(body.ToString(), System.Text.Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{GlobalSettings.AppSettings.MailServiceUrl}{EMAIL_SEND_NORMAL}"),
                    Content = _content,
                };

                using (var response = client.Send(request))
                {
                    response.EnsureSuccessStatusCode();
                    var strResponse = response.Content.ReadAsStringAsync().Result;

                    //_logger.LogInformation($"Send email to {toEmail} received: {strResponse}");

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        _logger.LogError($"Send email has error: {strResponse}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Send email has error: {toEmail} - {ex.Message} - {ex.StackTrace}");
            }

            return false;
        }

        private static readonly string EMAIL_SEND_WITH_TEMPLATE = "/SendMailWithTemplate";
        public static bool sendEmailTemplate(ILogger _logger, string toEmail, string title, string templateName, List<string> keyReplace, List<string> valueReplace)
        {
            try
            {
                var client = new HttpClient();

                var body = new JObject
                {
                    ["toEmail"] = toEmail,
                    ["title"] = title,
                    ["templateName"] = templateName,
                    ["keyReplace"] = JArray.FromObject(keyReplace),
                    ["valueReplace"] = JArray.FromObject(valueReplace)
                };

                var _content = new StringContent(body.ToString(), System.Text.Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{GlobalSettings.AppSettings.MailServiceUrl}{EMAIL_SEND_WITH_TEMPLATE}"),
                    Content = _content,
                };

                using (var response = client.Send(request))
                {
                    response.EnsureSuccessStatusCode();
                    var strResponse = response.Content.ReadAsStringAsync().Result;

                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Send email with template has error: {toEmail} - {ex.Message} - {ex.StackTrace}");
            }

            return false;
        }
    }
}
