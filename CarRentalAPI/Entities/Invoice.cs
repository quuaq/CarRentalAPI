using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalAPI.Entities
{
    public class Invoice
    {
        [Key, Column("Invoice_ID")]
        public int Invoice_ID { get; set; }
        public int Payment_ID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }

        [ForeignKey("Payment_ID")]
        public virtual Payment? Payment { get; set; }


    }
}
