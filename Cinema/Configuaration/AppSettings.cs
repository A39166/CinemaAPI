namespace CinemaAPI.Configuaration
{
    public class Database
    {
        public string DatabaseConfig { get; set; } = string.Empty;
    }
    public class AppSettings
    {
        public Database Database { get; set; }
        public string MailServiceUrl { get; set; }
        public string UploadPath { get; set; }
        public string ContentRootPath { get; set; }
    }
}
