using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalAPI.Entities
{
    public class Car
    {
        [Key, Column("CAr_ID")]
        public int Car_ID { get; set; }
        public string? Make { get; set; } // Araç Markası
        public string?  Model { get; set; }
        public int Year { get; set; } // Üretim yılı
        public decimal PricePerDay { get; set; } // Günlük Kiralama Ücreti
        public bool IsAvailable { get; set; } // Mevcut mu 
        public string? LicensePlate { get; set; } // Plaka Numarası
        public string? ImagePath { get; set; } 

    }
}
