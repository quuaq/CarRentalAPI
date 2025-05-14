namespace CarRentalAPI.DTO
{
    public class LogDTO
    {
        public string Action { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public string Details { get; set; } = null!;
        public string TcNo { get; set; } = null!;  
    }
}
