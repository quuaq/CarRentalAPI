using Microsoft.EntityFrameworkCore;
using CarRentalAPI.Entities;



namespace CarRentalAPI.DataAccess
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Car> Cars { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Payment> Payments { get; set; } 
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Role> Roles { get; set; }


    }
}
