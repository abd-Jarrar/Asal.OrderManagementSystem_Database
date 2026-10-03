using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Requests.ReservationRequests;

namespace Asal.OrderManagementSystem.Api.Interfaces
{
    public interface IReservationRepository
    {
        public Task<Guid> CreateReservationAsync(CreateReservationRequest request, CancellationToken ct);

        public Task<bool> CancelReservationAsync(Guid reservationId, CancellationToken ct);

        public Task<Reservation?> GetReservationByIdAsync(Guid reservationId, CancellationToken ct);
        public Task ExpireReservationsAsync(CancellationToken ct);
    
    }
}
