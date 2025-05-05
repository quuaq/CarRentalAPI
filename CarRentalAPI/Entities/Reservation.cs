using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalAPI.Entities
{
    public class Reservation
    {
        [Key, Column("Reservation_ID")]
        public int Reservation_ID { get; set; }
        public int User_ID { get; set; }
        public int Car_ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Status { get; set; }
        public int? Payment_ID { get; set; }

        [ForeignKey("User_ID")]
        public virtual User? User { get; set; }

        [ForeignKey("Car_ID")]
        public virtual Car? Car { get; set; }

        [ForeignKey("Payment_ID")]
        public virtual Payment? Payment { get; set; }
    }
}
