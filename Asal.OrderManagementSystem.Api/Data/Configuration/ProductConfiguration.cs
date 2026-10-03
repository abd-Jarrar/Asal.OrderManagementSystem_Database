using Asal.OrderManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asal.OrderManagementSystem.Api.Data.Configuration
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
            builder.HasData(
                new Product
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "Laptop",
                    SKU = "LAP-001",
                    Price = 899.99m,
                    StockQuantity = 15,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 5)
                },
                new Product
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Name = "Wireless Mouse",
                    SKU = "MOU-001",
                    Price = 29.99m,
                    StockQuantity = 50,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 12)
                },
                new Product
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    Name = "Mechanical Keyboard",
                    SKU = "KEY-001",
                    Price = 79.99m,
                    StockQuantity = 25,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 2, 10)
                },
                new Product
                {
                    Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    Name = "USB-C Cable",
                    SKU = "USB-001",
                    Price = 14.99m,
                    StockQuantity = 100,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 2, 20)
                },
                new Product
                {
                    Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    Name = "Monitor",
                    SKU = "MON-001",
                    Price = 249.99m,
                    StockQuantity = 8,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 3, 15)
                }
            );
        }
    }
}
