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
    public class ReservationRepository(AppDbContext _context,ILogger<ReservationRepository> _logger ) : IReservationRepository
    {
        private const int ReservationExpirationMinutes = 15;

        public async Task<bool> CancelReservationAsync(Guid reservationId, CancellationToken ct)
        {
            var reservation = await GetReservationByIdAsync(reservationId, ct);
            if (reservation is null)
                return false;
            if (reservation.Status != ReservationStatus.Active|| reservation.IsExpired)
                throw new InvalidOperationException("you can only cancel Active reservation");
            foreach (var reservationOrderItem in reservation.Items)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == reservationOrderItem.ProductId, ct);
                if (product is null )
                    throw new InvalidOperationException("Failed to cancel the order because it contains unavailable products");
                product.StockQuantity += reservationOrderItem.Quantity;
            }
            reservation.Status = ReservationStatus.Cancelled;
            await _context.SaveChangesAsync(ct);
            _logger.LogInformation("the reservation with id: {reservationId} was cancelled", reservationId);
            return true;

        }

        public async Task<Guid> ConvertReservationToOrder(Guid reservationId,CancellationToken ct)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(ct);

            var reservation = await GetReservationByIdAsync(reservationId, ct);

            if (reservation is null)
                throw new KeyNotFoundException($"reservation with the id {reservationId} was not found.");

            if (reservation.Status != ReservationStatus.Active || reservation.IsExpired)
                throw new InvalidOperationException("You can only convert an active reservation.");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = reservation.CustomerId,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>()
            };

            foreach (var reservationItem in reservation.Items)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(
                        p => p.Id == reservationItem.ProductId,
                        ct);

                if (product is null || !product.IsActive)
                {
                    throw new InvalidOperationException(
                        "Failed to convert the reservation because it contains unavailable products.");
                }

                order.OrderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = reservationItem.Quantity,
                    UnitPrice = reservationItem.UnitPrice
                });

            }

            reservation.Status = ReservationStatus.Converted;

            _context.Orders.Add(order);

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return order.Id;
        }
        public async Task<Guid> CreateReservationAsync(CreateReservationRequest request, CancellationToken ct)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == request.CustomerId, ct);

            if (customer is null)
                throw new KeyNotFoundException($"Customer with the id {request.CustomerId} was not found.");

            var now = DateTime.UtcNow;
            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                Status = ReservationStatus.Active,
                CreatedAt= now,
                ExpiresAt= now.AddMinutes(ReservationExpirationMinutes),
                Items = new List<ReservationItem>()
            };

            foreach (var item in request.Items)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, ct);

                if (product is null)
                    throw new KeyNotFoundException($"Product with the id {item.ProductId} was not found.");

                if (!product.IsActive)
                    throw new InvalidOperationException($"Product with the id {item.ProductId} is inactive.");

                if (item.Quantity > product.StockQuantity)
                {
                    _logger.LogWarning("There is no enough stock for product {ProductId}", product.Id);
                    throw new InvalidOperationException($"There is no enough stock for product {product.Id}.");

                }

                product.StockQuantity -= item.Quantity;
                var reservationItem = new ReservationItem
                {
                    Id = Guid.NewGuid(),
                    ReservationId = reservation.Id,
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                reservation.Items.Add(reservationItem);
            }

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync(ct);
            _logger.LogInformation("reservation with the id: {ReservationId} was created", reservation.Id);
            await transaction.CommitAsync(ct);
            return reservation.Id;
        }

        public async Task<Reservation?> GetReservationByIdAsync(Guid reservationId, CancellationToken ct)
        {
            return await _context.Reservations.Include(r =>r.Items).FirstOrDefaultAsync(r => r.Id == reservationId, ct);
        }

        

        public async Task<List<Reservation>> GetAllReservations(CancellationToken ct)
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
        public async Task RemoveReservationsItemsAsync(List<Reservation> expiredReservatoins, CancellationToken ct)
        {

            foreach (var reservation in expiredReservatoins)
            {
                
                foreach (var item in reservation.Items)
                {
                    var product = await _context.Products
                        .FirstAsync(p => p.Id == item.ProductId, ct);

                    product.StockQuantity += item.Quantity;
                }

                _context.ReservationItems.RemoveRange(reservation.Items);
                reservation.Status = ReservationStatus.Expired;
            }

            await _context.SaveChangesAsync(ct);
        }
    
    
    }
}
