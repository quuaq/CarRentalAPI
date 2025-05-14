using CarRentalAPI.DataAccess;
using CarRentalAPI.DTO;
using CarRentalAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly Context _context;

        public LogsController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetLogs()
        {
            var logs = await _context.Logs
                .Include(l => l.User)
                .Select(l => new
                {
                    l.Action,
                    l.Timestamp,
                    TcNo = l.User.TcNo,  // 🔥 Buraya dikkat!
                    l.Details
                })
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();

            return Ok(logs);
        }


    }
}
