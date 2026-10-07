using Asal.OrderManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asal.OrderManagementSystem.Api.Data.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Status).HasConversion<string>().IsRequired();
            builder.HasMany(o=>o.OrderItems).WithOne().HasForeignKey(oi => oi.OrderId);
            builder.Ignore(o => o.TotalAmount);
            builder.HasData(
                new Order
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    CustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Status = OrderStatus.Completed,
                    CreatedAt = new DateTime(2026, 3, 1)
                },
                new Order
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    CustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Status = OrderStatus.Completed,
                    CreatedAt = new DateTime(2026, 3, 10)
                },
                new Order
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    CustomerId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Status = OrderStatus.Completed,
                    CreatedAt = new DateTime(2026, 4, 5)
                },
                new Order
                {
                    Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    CustomerId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Status = OrderStatus.Pending,
                    CreatedAt = new DateTime(2026, 4, 15)
                },
                new Order
                {
                    Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                    CustomerId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Status = OrderStatus.Completed,
                    CreatedAt = new DateTime(2026, 5, 1)
                }
            );
        }
    }
}
