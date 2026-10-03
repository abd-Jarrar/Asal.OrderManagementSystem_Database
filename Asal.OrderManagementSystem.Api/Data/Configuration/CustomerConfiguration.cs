using Asal.OrderManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asal.OrderManagementSystem.Api.Data.Configuration
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Phone).HasMaxLength(13).IsRequired();
            builder.HasMany<Order>().WithOne().HasForeignKey(o => o.CustomerId).IsRequired();
            builder.Property(c => c.Name).HasMaxLength(50).IsRequired();
            builder.Property(c => c.Email).HasMaxLength(100).IsRequired();
            builder.HasData(
                new Customer
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Ahmad Ali",
                    Email = "ahmad@example.com",
                    Phone = "0599123456",
                    CreatedAt = new DateTime(2026, 1, 10)
                },
                new Customer
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Omar Khalil",
                    Email = "omar@example.com",
                    Phone = "0598765432",
                    CreatedAt = new DateTime(2026, 2, 15)
                },
                new Customer
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Sara Hassan",
                    Email = "sara@example.com",
                    Phone = "0599555444",
                    CreatedAt = new DateTime(2026, 3, 20)
                }
            );
        }
    }
}
