namespace CarRentalAPI.DTO
{
    public class InvoiceDTO
    {
        public int Invoice_ID { get; set; }
        public int Payment_ID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
