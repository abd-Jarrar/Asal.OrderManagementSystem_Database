using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Repositories;
using Asal.OrderManagementSystem.Api.Requests.ReservationRequests;
using Microsoft.EntityFrameworkCore;

namespace Asal.OrderManagementSystem.Api.Services
{
    public class ReservationService(IReservationRepository _reservationRepository,IProductRepository _productRepository, ILogger<ReservationService> _logger)
    {
        private const int ReservationExpirationMinutes = 15;
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
                CreatedAt = now,
                ExpiresAt = now.AddMinutes(ReservationExpirationMinutes),
                Items = new List<ReservationItem>()
            };

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetProductByIdAsync(item.ProductId, ct);

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

    }
}
