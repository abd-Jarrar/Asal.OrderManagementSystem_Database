using Asal.OrderManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asal.OrderManagementSystem.Api.Data.Configuration
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations");
            builder.HasKey(r => r.Id);
            builder.Property(o => o.Status).HasConversion<string>().IsRequired();
            builder.Ignore(r => r.IsExpired);
            builder.Property(r => r.CustomerId).IsRequired();
            builder.Property(r => r.CreatedAt).IsRequired();
            builder.Property(r => r.ExpiresAt).IsRequired();
            builder.Property(r => r.Status).IsRequired();
            builder.HasOne(r => r.Customer).WithMany()
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(r => r.Items).WithOne(ri => ri.Reservation)
                .HasForeignKey(ri => ri.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
