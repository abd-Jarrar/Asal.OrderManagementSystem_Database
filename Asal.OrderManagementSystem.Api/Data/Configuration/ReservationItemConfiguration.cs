using Asal.OrderManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asal.OrderManagementSystem.Api.Data.Configuration
{
    public class ReservationItemConfiguration : IEntityTypeConfiguration<ReservationItem>
    {
        public void Configure(EntityTypeBuilder<ReservationItem> builder)
        {
            builder.ToTable("ReservationItems", t =>
            {
                t.HasCheckConstraint("CK_ReservationItem_Price_MustBePositive", "[UnitPrice]>0");
                t.HasCheckConstraint("CK_ReservationItem_Quantity_MustBePositive", "[Quantity]>0");
            });

            builder.HasKey(ri => ri.Id);

            builder.Property(ri => ri.Quantity).IsRequired();

            builder.Property(ri => ri.UnitPrice)
               .HasColumnType("decimal(18,2)")
               .IsRequired();
            builder.HasOne(ri => ri.Product)
               .WithMany()
               .HasForeignKey(ri => ri.ProductId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
