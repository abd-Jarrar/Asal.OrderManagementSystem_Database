using Asal.OrderManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asal.OrderManagementSystem.Api.Data.Configuration
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

            builder.HasData(
                new OrderItem
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    OrderId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    ProductId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Quantity = 1,
                    UnitPrice = 899.99m
                },
                new OrderItem
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    OrderId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    ProductId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Quantity = 2,
                    UnitPrice = 29.99m
                },
                new OrderItem
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    OrderId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    ProductId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    Quantity = 1,
                    UnitPrice = 79.99m
                },
                new OrderItem
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                    OrderId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    ProductId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    Quantity = 3,
                    UnitPrice = 14.99m
                },
                new OrderItem
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                    OrderId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    ProductId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Quantity = 2,
                    UnitPrice = 899.99m
                },
                new OrderItem
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                    OrderId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    ProductId = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    Quantity = 1,
                    UnitPrice = 249.99m
                },
                new OrderItem
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000007"),
                    OrderId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    ProductId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Quantity = 2,
                    UnitPrice = 29.99m
                },
                new OrderItem
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000008"),
                    OrderId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                    ProductId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    Quantity = 2,
                    UnitPrice = 79.99m
                },
                new OrderItem
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000009"),
                    OrderId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                    ProductId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    Quantity = 5,
                    UnitPrice = 14.99m
                }
            );

        }
    }
}
