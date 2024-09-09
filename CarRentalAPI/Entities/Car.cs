namespace CarRentalAPI.Entities
{
    public class Car
    {
        public int ID { get; set; }
        public string? Make { get; set; } // Araç Markası
        public string?  Model { get; set; }
        public int Year { get; set; } // Üretim yılı
        public decimal PricePerDay { get; set; } // Günlük Kiralama Ücreti
        public bool IsAvailable { get; set; } // Mevcut mu 
        public string? LicensePlate { get; set; } // Plaka Numarası

    }
}
