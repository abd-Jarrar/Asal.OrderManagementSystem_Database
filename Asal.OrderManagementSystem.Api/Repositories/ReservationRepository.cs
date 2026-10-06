using Asal.OrderManagementSystem.Api.Data;
using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Requests.OrderItemRequests;
using Asal.OrderManagementSystem.Api.Requests.OrderRequests;
using Asal.OrderManagementSystem.Api.Requests.ReservationRequests;
using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace Asal.OrderManagementSystem.Api.Repositories
{
    public class ReservationRepository(AppDbContext _context) : IReservationRepository
    {
        
        public async Task<Reservation?> GetReservationByIdAsync(Guid reservationId, CancellationToken ct)
        {
            return await _context.Reservations.Include(r =>r.Items).FirstOrDefaultAsync(r => r.Id == reservationId, ct);
        }

        public void RemoveRangeOfItems(List<ReservationItem> items)
        {
            _context.ReservationItems.RemoveRange(items);
        }
        public async Task<List<Reservation>> GetAllReservationsAsync(CancellationToken ct)
        {
            return await _context.Reservations.Include(r => r.Items).ToListAsync(ct);
        }

        public async Task<List<Reservation>> GetReservationsPendingExpirationAsync(CancellationToken ct)
        {
            return await _context.Reservations.Include(r => r.Items).
                Where(r => (DateTime.UtcNow >= r.ExpiresAt)&&(r.Status==ReservationStatus.Active))
                .ToListAsync(ct);
        }

        public async Task<List<Reservation>> GetExpiredReservationsAsync(CancellationToken ct)
        {
            return await _context.Reservations.Include(r => r.Items).Where(r=>r.Status==ReservationStatus.Expired).ToListAsync(ct);
        }
        public async Task AddReservationAsync(Reservation reservation,CancellationToken ct)
        {
            await _context.Reservations.AddAsync(reservation,ct);
        }

        
    }
}
