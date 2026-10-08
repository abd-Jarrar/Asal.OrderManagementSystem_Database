using Asal.OrderManagementSystem.Api.Data.Configuration;
using Asal.OrderManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Asal.OrderManagementSystem.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext>options):DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem>Items { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<ReservationItem> ReservationItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerConfiguration).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
