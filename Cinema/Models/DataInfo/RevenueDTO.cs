namespace CinemaAPI.Models.DataInfo
{
    public class RevenueByMoviesDTO
    {
        public string MovieName { get; set; }
        public int TotalTicketSell { get; set; }
        public double TotalRevenue { get; set; }
    }

    public class RevenueByCinemasDTO
    {
        public string CinemaName { get; set; }
        public int TotalTicketSell { get; set; }
        public double TotalRevenue { get; set; }
    }
}
