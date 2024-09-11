using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalAPI.Entities
{
    public class User
    {
        [Key, Column("User_ID")]
        public int User_ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? PhoneNumber { get; set; }
        public int Role_ID { get; set; }
        public DateTime CreatedDate { get; set; } // Hesap olıuşturma tarihi

        [ForeignKey("Role_ID")]
        public virtual Role? Role { get; set; } // Foreing Key ilişkisi tanımlama


    }
}
