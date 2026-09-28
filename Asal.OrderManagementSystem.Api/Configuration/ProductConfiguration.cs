using Asal.OrderManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asal.OrderManagementSystem.Api.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products",
                t => {
                t.HasCheckConstraint("CK_Products_Price_Positive", "[Price] > 0");
                t.HasCheckConstraint("CK_Products_StockQuantity_NotNegatives", "[StockQuantity] >= 0");
                });
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).HasMaxLength(100);
            builder.Property(p => p.SKU).HasMaxLength(50);
            builder.HasIndex(p => p.SKU).IsUnique();
            builder.Property(p => p.Price).IsRequired();
            builder.Property(p => p.IsActive).IsRequired();
            builder.HasMany<OrderItem>().WithOne().HasForeignKey(oi => oi.ProductId);

        }
    }
}
