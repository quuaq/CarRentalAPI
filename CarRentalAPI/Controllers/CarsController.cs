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
                    (r.Status == "Paid"))
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
            if(car == null)
            {
                return NotFound();
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
                        (r.IsTemporary && r.ExpireDate > DateTime.UtcNow)
                    ) &&
                    // Tarih çakışmasını kontrol et
                    !(r.EndDate <= start || r.StartDate >= end)
                ))
                .ToListAsync();

            return Ok(availableCars);
        }


    }
}
