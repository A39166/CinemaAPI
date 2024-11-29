using System.ComponentModel;

namespace CinemaAPI.Models.DataInfo
{
    public class BillDetailCLientDTO : BaseDTO
    {
        public ShortMoviesDTO Movie { get; set; }
        public ShortScreenDTO Screen { get; set; }
        public ShortCinemaDTO Cinema { get; set; }
        public List<SeatBillDTO> Seat { get; set; }
        public List<ComboForBill> Combo {  get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string  LanguageTypeName { get; set; }
        public string CouponUuid {  get; set; }
        public double TotalSeatPrice { get; set; }
        public double TotalComboPrice { get; set; }
        public double PayPrice { get; set; }
        public string QRPath { get; set; }
        public sbyte Status { get; set; }
    }
    
}
