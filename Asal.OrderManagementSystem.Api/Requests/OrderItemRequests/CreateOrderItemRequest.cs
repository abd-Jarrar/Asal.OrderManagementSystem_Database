namespace Asal.OrderManagementSystem.Api.Requests.OrderItemRequests
{

    public class CreateOrderItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
