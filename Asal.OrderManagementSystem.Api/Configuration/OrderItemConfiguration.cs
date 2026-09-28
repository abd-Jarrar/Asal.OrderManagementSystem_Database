using Asal.OrderManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asal.OrderManagementSystem.Api.Configuration
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems", t =>
            {
                t.HasCheckConstraint("CK_OrderItem_Price_MustBePositive", "[UnitPrice]>0");
                t.HasCheckConstraint("CK_OrderItem_Quantity_MustBePositive", "[Quantity]>0");
            });
            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.ProductId).IsRequired();
            builder.Property(oi => oi.OrderId).IsRequired();
            
            

        }
    }
}
