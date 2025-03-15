namespace CarRentalAPI.DTO
{
    public class PaymentDTO
    {
        public int Payment_ID { get; set; }
        public int Reservation_ID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }

        // Sadece temel reservation bilgileri
        public int? User_ID { get; set; }
        public int? Car_ID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Status { get; set; }
    }
}
