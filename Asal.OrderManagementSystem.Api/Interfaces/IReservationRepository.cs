using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Requests.ReservationRequests;

namespace Asal.OrderManagementSystem.Api.Interfaces
{
    public interface IReservationRepository
    {
        public Task<Guid> CreateReservationAsync(CreateReservationRequest request, CancellationToken ct);

        public Task<bool> CancelReservationAsync(Guid reservationId, CancellationToken ct);

        public Task<Reservation?> GetReservationByIdAsync(Guid reservationId, CancellationToken ct);
        public Task RemoveReservationsItemsAsync(List<Reservation> expiredReservatoins, CancellationToken ct);

        public Task<List<Reservation>> GetExpiredReservationsAsync(CancellationToken ct);

        public Task<List<Reservation>> GetAllReservations(CancellationToken ct);

        public Task<bool> ConvertReservationToOrder(Guid reservationId, CancellationToken ct);
    }
}
