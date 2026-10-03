namespace Asal.OrderManagementSystem.Api.Requests.OrderItemRequests
{
    public class AddOrderItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity {  get; set; }
    }
}
