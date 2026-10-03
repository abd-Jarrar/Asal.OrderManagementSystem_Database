using Asal.OrderManagementSystem.Api.Requests.OrderItemRequests;

namespace Asal.OrderManagementSystem.Api.Requests.OrderRequests
{
    public class CreateOrderRequest
    {
        public Guid CustomerId { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }
}
