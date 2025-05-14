using CarRentalAPI.DataAccess;
using Microsoft.EntityFrameworkCore;

public class ReservationCleanerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReservationCleanerService> _logger;

    public ReservationCleanerService(IServiceScopeFactory scopeFactory, ILogger<ReservationCleanerService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Context>();

                
                var expiredReservations = await context.Reservations
                    .Where(r => r.IsTemporary && r.ExpireDate < DateTime.UtcNow)
                    .ToListAsync();

                if (expiredReservations.Any())
                {
                    context.Reservations.RemoveRange(expiredReservations);
                    await context.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation($"{expiredReservations.Count} expired reservations cleaned.");
                }

                
                var completedReservations = await context.Reservations
                    .Where(r => r.Status == "Paid" && r.EndDate < DateTime.UtcNow)
                    .ToListAsync();

                foreach (var reservation in completedReservations)
                {
                    reservation.Status = "Completed";
                }

                if (completedReservations.Any())
                {
                    await context.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation($"{completedReservations.Count} reservations marked as completed.");
                }
            }

            
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
        }
    }
}
