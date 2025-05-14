using CarRentalAPI.DataAccess;
using CarRentalAPI.DTO;
using CarRentalAPI.Entities;
using CarRentalAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly Context _context;
        private readonly LogService _logService;

        public PaymentsController(Context context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        // bring all payments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDTO>>> GetPayments()
        {
            var payments = await _context.Payments
                .Include(p => p.Reservation)
                .ThenInclude(r => r.User)
                .Include(p => p.Reservation.Car)
                .Select(p => new PaymentDTO
                {
                    Payment_ID = p.Payment_ID,
                    Reservation_ID = p.Reservation_ID,
                    PaymentDate = p.PaymentDate,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus,

                    // Reservation bilgileri
                    User_ID = p.Reservation.User_ID,
                    Car_ID = p.Reservation.Car_ID,
                    StartDate = p.Reservation.StartDate,
                    EndDate = p.Reservation.EndDate,
                    TotalPrice = p.Reservation.TotalPrice,
                    Status = p.Reservation.Status
                })
                .ToListAsync();

            return payments;
        }


        // get details of a specific payment
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDTO>> GetPayment(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.User) // Kullanıcıyı dahil et
                .Include(p => p.Reservation.Car) // Arabayı dahil et
                .Where(p => p.Payment_ID == id)
                .Select(p => new PaymentDTO
                {
                    Payment_ID = p.Payment_ID,
                    Reservation_ID = p.Reservation_ID,
                    PaymentDate = p.PaymentDate,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus,
                    User_ID = p.Reservation.User_ID,
                    Car_ID = p.Reservation.Car_ID,
                    StartDate = p.Reservation.StartDate,
                    EndDate = p.Reservation.EndDate,
                    TotalPrice = p.Reservation.TotalPrice,
                    Status = p.Reservation.Status
                })
                .FirstOrDefaultAsync();

            if (payment == null)
            {
                return NotFound();
            }

            return payment;
        }


        [HttpPost]
        public async Task<ActionResult<PaymentDTO>> CreatePayment(PaymentDTO paymentDTO)
        {
            if (paymentDTO.PaymentDate < DateTime.UtcNow)
            {
                return BadRequest("The payment date cannot be an expired date!");
            }

            var reservation = await _context.Reservations.FindAsync(paymentDTO.Reservation_ID);
            if (reservation == null)
            {
                return NotFound("No connected reservation found.");
            }

            if (paymentDTO.PaymentStatus == "Paid")
            {
                reservation.Status = "Paid";
                reservation.IsTemporary = false; // Geçici rezervasyon değil
                reservation.ExpireDate = null; // Geçerlilik süresi yok
            }

            var payment = new Payment
            {
                Reservation_ID = paymentDTO.Reservation_ID,
                PaymentDate = paymentDTO.PaymentDate,
                Amount = paymentDTO.Amount,
                PaymentMethod = paymentDTO.PaymentMethod,
                PaymentStatus = paymentDTO.PaymentStatus,
                Reservation = reservation
            };

            _logService.AddLog(reservation.User_ID, "Payment Created", $"Amount: {payment.Amount}, Status: {payment.PaymentStatus}");

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(); // Önce payment'ı kaydet → ID oluşsun

            // 🔧 Eksik olan yer: Rezervasyon kaydına Payment_ID'yi yaz
            reservation.Payment_ID = payment.Payment_ID;
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync(); // Güncellenmiş rezervasyonu kaydet

            // ✅ Fatura otomatik oluşturuluyor
            var invoice = new Invoice
            {
                Payment_ID = payment.Payment_ID,
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = payment.Amount
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            var createdPaymentDTO = new PaymentDTO
            {
                Payment_ID = payment.Payment_ID,
                Reservation_ID = payment.Reservation_ID,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                User_ID = reservation.User_ID,
                Car_ID = reservation.Car_ID,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                TotalPrice = reservation.TotalPrice,
                Status = reservation.Status
            };

            return CreatedAtAction(nameof(GetPayments), new { id = payment.Payment_ID }, createdPaymentDTO);
        }





        // ödeme güncelle (ödeme durumu değiştirilebilir)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, PaymentDTO paymentDTO)
        {
            if (id != paymentDTO.Payment_ID)
            {
                return BadRequest("Payment ID in URL and body must match.");
            }

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound("Payment not found.");
            }

            // Güncelleme işlemi
            payment.PaymentDate = paymentDTO.PaymentDate;
            payment.Amount = paymentDTO.Amount;
            payment.PaymentMethod = paymentDTO.PaymentMethod;
            payment.PaymentStatus = paymentDTO.PaymentStatus;

            _context.Entry(payment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }



        // ödeme silme (opsiyonel ve test amaçlı)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if(payment == null)
            {
                return NotFound();
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ödeme durumu güncelleme (demo ödeme işlemleri)
        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, string status)
        {
            var payment = await _context.Payments.Include(p => p.Reservation)
                                                 .FirstOrDefaultAsync(p => p.Payment_ID == id);
            if (payment == null)
            {
                return NotFound();
            }

            // Geçerli ödeme statüleri
            var validStatuses = new List<string> { "Ödendi", "Beklemede", "İptal" };
            if (!validStatuses.Contains(status))
            {
                return BadRequest("Geçersiz ödeme durumu!");
            }

            // Ödeme durumunu güncelle
            payment.PaymentStatus = status;

            // Eğer ödeme başarılıysa, rezervasyonu "Onaylandı" yap
            if (status == "Ödendi" && payment.Reservation != null)
            {
                payment.Reservation.Status = "Onaylandı";
            }
            // Eğer ödeme iptal edilirse, rezervasyon "İptal Edildi" olarak güncellenir
            else if (status == "İptal" && payment.Reservation != null)
            {
                payment.Reservation.Status = "İptal Edildi";
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(p => p.Payment_ID == id);
        }

    }
}
