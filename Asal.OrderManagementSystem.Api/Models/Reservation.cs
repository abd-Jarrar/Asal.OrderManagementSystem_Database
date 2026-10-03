namespace Asal.OrderManagementSystem.Api.Models
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        public bool IsExpired =>DateTime.UtcNow >= ExpiresAt;
        public Customer Customer { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public ReservationStatus Status { get; set; }

        public List<ReservationItem> Items { get; set; }=new List<ReservationItem>();
    }
}
