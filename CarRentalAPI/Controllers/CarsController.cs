using CarRentalAPI.DataAccess;
using CarRentalAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly Context _context;

        //Constructor ile Context sınıfını enjekte ediyoruz
        public CarsController(Context context)
        {
            _context = context;
        }

        // 1. Get: api/Cars ==> Rezervasyon durumuna göre tüm araçları listelemeye yarar
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            var currentDate = DateTime.UtcNow;
            var reservedCarIds = await _context.Reservations
                .Where(r =>
                    (r.Status == "Pending" && r.ExpireDate > currentDate && r.IsTemporary) ||
                    (r.Status == "Paid" && r.EndDate > currentDate)) // Paid ve hâlâ kiradaysa
                .Select(r => r.Car_ID)
                .ToListAsync();

            return await _context.Cars
                .Where(c => !reservedCarIds.Contains(c.Car_ID) && c.IsAvailable)
                .ToListAsync();
        }


        // 2.Get: api/Cars/5 ==> Belirli bir aracı ID'ye göre getirme
        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            return car;
        }

        //3. Post: api/Cars ==> Yeni Araç Ekleme
        [HttpPost]
        public async Task<ActionResult<Car>> CreateCar(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCar), new { id = car.Car_ID }, car);
        }

        //4. Put: api/Cars/5 ==> Mevcut bir aracı güncelleme 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(int id, Car car)
        {
            if (id != car.Car_ID)
            {
                return BadRequest();
            }
            _context.Entry(car).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        //5. Delete: api/Cars/5 ==> Belirli bir aracı silme
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound("Car not found.");
            }

            // Aktif rezervasyon kontrolü
            bool hasActiveReservation = await _context.Reservations.AnyAsync(r =>
                r.Car_ID == id &&
                (r.Status == "Pending" || r.Status == "Paid")
            );

            if (hasActiveReservation)
            {
                return BadRequest("This car cannot be deleted because it has active reservations.");
            }

            // Eğer resim dosyası varsa onu da silelim
            if (!string.IsNullOrEmpty(car.ImagePath))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", car.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // aracın plakasına göre aracı silme
        [HttpDelete("by-plate/{plate}")]
        public async Task<IActionResult> DeleteCarByPlate(string plate)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.LicensePlate == plate);
            if (car == null)
            {
                return NotFound("Car not found with this license plate.");
            }

            bool hasActiveReservation = await _context.Reservations.AnyAsync(r =>
                r.Car_ID == car.Car_ID &&
                (r.Status == "Pending" || r.Status == "Paid")
            );

            if (hasActiveReservation)
            {
                return BadRequest("This car is currently under active reservation or has been paid for.");
            }

            // Resim silme ve araba silme işlemi devam eder...
            if (!string.IsNullOrEmpty(car.ImagePath))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", car.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        //CarExists metodunu tanımlayalım. Aracın var olup olmadığını kontrol eder.
        private bool CarExists(int id)
        {
            return _context.Cars.Any(e=>e.Car_ID == id);
        }


        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableCars(DateTime start, DateTime end)
        {
            start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
            end = DateTime.SpecifyKind(end, DateTimeKind.Utc);

            var availableCars = await _context.Cars
                .Where(car => !_context.Reservations.Any(r =>
                    r.Car_ID == car.Car_ID &&
                    (
                        r.Status == "Paid" ||
                        (r.Status == "Pending" && r.ExpireDate > DateTime.UtcNow)
                    ) &&
                    r.StartDate < end && r.EndDate > start

                ))
                .ToListAsync();

            return Ok(availableCars);
        }

        // resim yükleme yeri
        [HttpPost("upload-image/{id}")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();

            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                car.ImagePath = $"/images/{uniqueFileName}";
                await _context.SaveChangesAsync();

                return Ok(new { imagePath = car.ImagePath });
            }

            return BadRequest("Invalid file.");
        }





    }
}
