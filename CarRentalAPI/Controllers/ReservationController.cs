﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CarRentalAPI.DataAccess;
using CarRentalAPI.Entities;
using Microsoft.EntityFrameworkCore;
using CarRentalAPI.DTO;
using CarRentalAPI.Services;


namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly Context _context;
        private readonly LogService _logService;

        public ReservationController(Context context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        // Admin için tüm reservationları listeleme
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Car)
                .ToListAsync();
        }

        // Belirli bir rezervasyonu getirme
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Reservation_ID == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        // Yeni rezervasyon oluşturma
        [HttpPost]
        public async Task<ActionResult<ReservationDTO>> CreateReservation(ReservationDTO reservationDto)
        {
            var reservation = new Reservation
            {
                User_ID = reservationDto.User_ID,
                Car_ID = reservationDto.Car_ID,
                StartDate = reservationDto.StartDate,
                EndDate = reservationDto.EndDate,
                TotalPrice = reservationDto.TotalPrice,
                Status = reservationDto.Status,
                IsTemporary = true, // Geçici rezervasyon
                ExpireDate = DateTime.UtcNow.AddMinutes(2) // 15 dakika geçerli
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            // rezervasyon oluşturulduğuna dair log
            _logService.AddLog(reservation.User_ID, "Reservation Created", $"Car ID: {reservation.Car_ID}, Total Price: {reservation.TotalPrice}");

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Reservation_ID }, reservationDto);
        }


        // Rezarvasyon güncelleme
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, ReservationDTO reservationDto)
{
        if (id != reservationDto.Reservation_ID)
        {
            return BadRequest("ID uyuşmazlığı!");
        }

        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
        {
            return NotFound();
        }

            // Güncellenen verileri DTO'dan asıl modele aktarıyoruz
            reservation.User_ID = reservationDto.User_ID;
            reservation.Car_ID = reservationDto.Car_ID;
            reservation.StartDate = reservationDto.StartDate;
            reservation.EndDate = reservationDto.EndDate;
            reservation.TotalPrice = reservationDto.TotalPrice;
            reservation.Status = reservationDto.Status;

            _context.Entry(reservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
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


        // Rezervasyon silme
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            // silme için log
            _logService.AddLog(reservation.User_ID, "Reservation Deleted", $"Reservation ID: {reservation.Reservation_ID}, Car ID: {reservation.Car_ID}");

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Kullanıcının tüm rezervasyonlarını getirme
        //[HttpGet("user/{userId}")]
        //public async Task<ActionResult<IEnumerable<Reservation>>> GetReservationsByUser(int userId)
        //{
        //    return await _context.Reservations
        //        .Where(r => r.User_ID == userId)
        //        .Include(r => r.Car)
        //        .Include(r => r.User)
        //        .ToListAsync();
        //}

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservationsByUser(int userId)
        {
            var reservations = await _context.Reservations
                .Where(r => r.User_ID == userId)
                .Include(r => r.Car)
                .Include(r => r.Payment)
                .Select(r => new ReservationDTO
                {
                    Reservation_ID = r.Reservation_ID,
                    User_ID = r.User_ID,
                    Car_ID = r.Car_ID,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    TotalPrice = r.TotalPrice,
                    Status = r.Status,
                    CarName = r.Car != null ? r.Car.Make + " " + r.Car.Model : "Bilinmeyen Araç",
                    Payment_ID = r.Payment != null ? r.Payment.Payment_ID : null // 🔥 Burası eklendi
                })
                .ToListAsync();

            return reservations;
        }


        // Rezervasyon var mı kontrolü
        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Reservation_ID == id);
        }
    }//
}