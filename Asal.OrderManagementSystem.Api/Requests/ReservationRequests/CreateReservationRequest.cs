using Asal.OrderManagementSystem.Api.Requests.OrderItemRequests;
using Asal.OrderManagementSystem.Api.Requests.ReservationItemRequests;

namespace Asal.OrderManagementSystem.Api.Requests.ReservationRequests
{
    public class CreateReservationRequest
    {
        public Guid CustomerId { get; set; }
        public List<CreateReservationItemRequest> Items { get; set; } = new();

    }
}
