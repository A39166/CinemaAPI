using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CinemaAPI.Models.BaseRequest;

namespace CinemaAPI.Models.Request
{
    public class CreateBillRequest 
    {
        public string ShowtimeUuid { get; set; }
        public string? CouponUuid {  get; set; }
        public double TotalSeatPrice { get; set; }
        public double TotalComboPrice { get; set; }
        public double PayPrice { get; set; }
        public List<TicketForm> Seats { get; set; }
        public List<TicketComboForm> Combo {  get; set; }
        [DefaultValue(1)]
        public sbyte Status {  get; set; }

    }
    public class TicketForm
    {
        public string SeatUuid { get; set; }
        public string SeatPriceUuid { get; set; }
    }
    public class TicketComboForm
    {
        public string ComboUuid { get; set; }
        public int Quantity { get; set; }
    }
}
