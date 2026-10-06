using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Requests.ReservationRequests;
using Microsoft.EntityFrameworkCore;

namespace Asal.OrderManagementSystem.Api.Interfaces
{
    public interface IReservationRepository
    {

        

        public  Task AddReservationAsync(Reservation reservation, CancellationToken ct);

        public void RemoveRangeOfItems(List<ReservationItem> items);
        public Task<Reservation?> GetReservationByIdAsync(Guid reservationId, CancellationToken ct);

        public Task<List<Reservation>> GetExpiredReservationsAsync(CancellationToken ct);

        public Task<List<Reservation>> GetAllReservationsAsync(CancellationToken ct);

        
        public Task<List<Reservation>> GetReservationsPendingExpirationAsync(CancellationToken ct);
    }
}
