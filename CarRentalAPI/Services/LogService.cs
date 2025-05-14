using CarRentalAPI.DataAccess;
using CarRentalAPI.Entities;

namespace CarRentalAPI.Services
{
    public class LogService
    {
        private readonly Context _context;

        public LogService(Context context)
        {
            _context = context;
        }
        // _logService -> Yazıyor.
        // LogController -> Okuyor.
        public void AddLog(int userId, string action, string? details = null)
        {
            var log = new Log
            {
                User_ID = userId,
                Action = action,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            _context.Logs.Add(log);
            _context.SaveChanges();
        }
    }
}
