namespace CarRentalAPI.DTO
{
    public class InvoiceDetailDTO
    {
        public int Invoice_ID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }

        // Payment bilgileri
        public int Payment_ID { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentMethod { get; set; }

        // Reservation bilgileri
        public int Reservation_ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status { get; set; }

        // Car bilgileri
        public string? CarName { get; set; }

        // User bilgileri
        public string? FullName { get; set; }
        public string? Email { get; set; }
    }

}
