namespace Asal.OrderManagementSystem.Api.Models
{
    public class ReservationItem
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }

        public Reservation Reservation { get; set; } = null!;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
