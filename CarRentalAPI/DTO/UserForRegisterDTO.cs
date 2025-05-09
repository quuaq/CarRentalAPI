namespace CarRentalAPI.DTO
{
    public class UserForRegisterDTO
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string TcNo { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? LicenseNumber { get; set; } = null!;
    }
}
