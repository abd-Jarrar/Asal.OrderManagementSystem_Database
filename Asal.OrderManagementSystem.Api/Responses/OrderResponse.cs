using Asal.OrderManagementSystem.Api.Models;

namespace Asal.OrderManagementSystem.Api.Responses
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        private OrderResponse()
        {
            
        }
        public static OrderResponse FromModel(Order order)
        {
            
            return new OrderResponse
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
            };
        }

        public static List<OrderResponse> FromModels(List<Order> orders)
        {
            return orders.Select(o=>OrderResponse.FromModel(o)).ToList();
        }
    }
}
