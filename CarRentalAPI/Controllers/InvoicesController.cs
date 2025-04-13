using CarRentalAPI.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalAPI.Entities;
using CarRentalAPI.DTO;


namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly Context _context;

        public InvoicesController(Context context)
        {
            _context = context;
        }

        // tüm faturaları getir
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceDTO>>> GetInvoices()
        {
            var invoices = await _context.Invoices.Include(i => i.Payment)
                                                  .Select(i => new InvoiceDTO
                                                  {
                                                      Invoice_ID = i.Invoice_ID,
                                                      Payment_ID = i.Payment_ID,
                                                      InvoiceDate = i.InvoiceDate,
                                                      TotalAmount = i.TotalAmount
                                                  }).ToListAsync();
            return invoices;
        }

        // id'ye göre faturaları getir
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDTO>> GetInvoice(int id)
        {
            var invoice = await _context.Invoices.Include(i => i.Payment)
                                                 .Where(i => i.Invoice_ID == id)
                                                 .Select(i => new InvoiceDTO
                                                 {
                                                     Invoice_ID = i.Invoice_ID,
                                                     Payment_ID = i.Payment_ID,
                                                     InvoiceDate = i.InvoiceDate,
                                                     TotalAmount = i.TotalAmount
                                                 }).FirstOrDefaultAsync();
            if (invoice == null)
            {
                return NotFound();
            }

            return invoice;
        }

        // yeni fatura oluştur
        [HttpPost]
        public async Task<ActionResult<InvoiceDTO>> CreateInvoice(InvoiceDTO invoiceDTO)
        {
            var payment = await _context.Payments.FindAsync(invoiceDTO.Payment_ID);
            if (payment == null)
            {
                return NotFound("Ödeme Bulunamadı!");
            }

            var invoice = new Invoice
            {
                Payment_ID = invoiceDTO.Payment_ID,
                InvoiceDate = invoiceDTO.InvoiceDate,
                TotalAmount = invoiceDTO.TotalAmount
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            invoiceDTO.Invoice_ID = invoice.Invoice_ID; // oluşturulan fatura ID'sini geri döndür
            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Invoice_ID }, invoiceDTO);
        }

        // fatura güncelle 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, InvoiceDTO invoiceDTO)
        {
            if (id != invoiceDTO.Invoice_ID)
            {
                return BadRequest("Fatura ID'leri uyuşmuyor!");
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound("Fatura Bulunamadı!");
            }

            invoice.Payment_ID = invoiceDTO.Payment_ID;
            invoice.InvoiceDate = invoiceDTO.InvoiceDate;
            invoice.TotalAmount = invoiceDTO.TotalAmount;

            _context.Entry(invoice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(id))
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

        // fatural sil 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.Invoice_ID == id);
        }
    }
}
