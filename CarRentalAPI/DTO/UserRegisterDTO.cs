namespace CarRentalAPI.DTO
{
    public class UserRegisterDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public int Role_ID { get; set; }  // Varsayılan olarak 2 (normal kullanıcı) atanabilir
    }
}
