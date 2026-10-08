namespace Asal.OrderManagementSystem.Api.Requests.ReservationItemRequests
{
    public class CreateReservationItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
