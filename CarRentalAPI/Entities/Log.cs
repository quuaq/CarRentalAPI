using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CarRentalAPI.Entities
{
    public class Log
    {
        [Key, Column("Log_ID")]
        public int Log_ID { get; set; }
        public int User_ID { get; set; }
        public string Action { get; set; } = null!;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Details { get; set; }

        [ForeignKey("User_ID")]
        public virtual User? User { get; set; }
    }
}
