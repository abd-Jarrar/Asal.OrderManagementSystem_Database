namespace Asal.OrderManagementSystem.Api.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public decimal TotalAmount { get; set; }

    }
}
