namespace CarRentalAPI.DTO
{
    public class ReservationDTO
    {
        public int Reservation_ID { get; set; }
        public int User_ID { get; set; }
        public int Car_ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Status { get; set; }
    }
}
