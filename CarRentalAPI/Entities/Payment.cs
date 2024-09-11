using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalAPI.Entities
{
    public class Payment
    {
        [Key, Column("Payment_ID")]
        public int Payment_ID { get; set; }
        public int Reservation_ID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }

        [ForeignKey("Reservation_ID")]
        public virtual Reservation? Reservation { get; set; }

    }
}
