namespace CarRentalAPI.DTO
{
    public class UserDTO
    {
        public int User_ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int Role_ID { get; set; }  // RoleName yerine sadece Role_ID kullan
        public DateTime CreatedDate { get; set; }
        public string? Password { get; set; }
    }
}
