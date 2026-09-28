using Asal.OrderManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asal.OrderManagementSystem.Api.Configuration
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
        }
    }
}
