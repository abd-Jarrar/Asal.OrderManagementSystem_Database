using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Requests.ReservationItemRequests;
using Asal.OrderManagementSystem.Api.Requests.ReservationRequests;
namespace Asal.OrderManagementSystem.Api.Services
{
    public class ReservationService(
        IReservationRepository _reservationRepository,
        IProductRepository _productRepository,
        ICustomerRepository _customerRepository,
        IOrderRepository _orderRepository,
        IUnitOfWork _unitOfWork,
        ILogger<ReservationService> _logger)
    {
        private const int ReservationExpirationMinutes = 15;

        private Reservation CreateReservation(CreateReservationRequest request)
        {
            var now = DateTime.UtcNow;

            return new Reservation
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                Status = ReservationStatus.Active,
                CreatedAt = now,
                ExpiresAt = now.AddMinutes(ReservationExpirationMinutes),
                Items = new List<ReservationItem>()
            };
        }
        public async Task<Guid> CreateReservationAsync(CreateReservationRequest request,CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                await EnsureCustomerExistsAsync(request.CustomerId, ct);

                var reservation = CreateReservation(request);

                await AddReservationItemsAsync(reservation, request.Items, ct);

                await _reservationRepository.AddReservationAsync(reservation,ct);

                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitTransactionAsync(ct);

                _logger.LogInformation(
                    "Reservation with id {ReservationId} was created",
                    reservation.Id);

                return reservation.Id;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(ct);
                throw;
            }
        }
        private async Task AddReservationItemsAsync(
    Reservation reservation,
    IEnumerable<CreateReservationItemRequest> items,
    CancellationToken ct)
        {
            foreach (var item in items)
            {
                var product = await _productRepository
                    .GetProductByIdAsync(item.ProductId, ct);

                if (product is null)
                    throw new KeyNotFoundException(
                        $"Product with the id {item.ProductId} was not found.");

                if (!product.IsActive)
                    throw new InvalidOperationException(
                        $"Product with the id {item.ProductId} is inactive.");

                if (item.Quantity > product.StockQuantity)
                {
                    _logger.LogWarning(
                        "There is not enough stock for product {ProductId}",
                        product.Id);

                    throw new InvalidOperationException(
                        $"There is not enough stock for product {product.Id}.");
                }

                product.StockQuantity -= item.Quantity;

                reservation.Items.Add(new ReservationItem
                {
                    Id = Guid.NewGuid(),
                    ReservationId = reservation.Id,
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });
            }
        }
        private async Task EnsureCustomerExistsAsync(Guid customerId,CancellationToken ct)
        {
            var customer = await _customerRepository
                .GetCustomerByIdAsync(customerId, ct);

            if (customer is null)
                throw new KeyNotFoundException(
                    $"Customer with the id {customerId} was not found.");
        }
        public async Task CancelReservationAsync(Guid reservationId,CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var reservation = await GetActiveReservationAsync(reservationId, ct);
                foreach (var reservationItem in reservation.Items)
                {
                    var product =
                        await _productRepository.GetProductByIdAsync(reservationItem.ProductId,ct);
                    if (product is null)
                        throw new InvalidOperationException(
                            "Failed to cancel the reservation because it contains unavailable products.");
                     _productRepository.IncreaseStock(product, reservationItem.Quantity);
                }
                reservation.Status = ReservationStatus.Cancelled;
                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitTransactionAsync(ct);
                _logger.LogInformation(
                    "Reservation with id {ReservationId} was cancelled",
                    reservationId);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(ct);
                throw;
            }
        }
        private static bool IsReservationActive(Reservation reservation)
        {
            return reservation.Status == ReservationStatus.Active &&
           !reservation.IsExpired;
        }

        public async Task<Guid> ConvertReservationToOrderAsync(Guid reservationId,CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var reservation =await GetActiveReservationAsync(reservationId, ct);

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = reservation.CustomerId,
                    Status = OrderStatus.Pending,
                    OrderItems = new List<OrderItem>()
                };

                foreach (var reservationItem in reservation.Items)
                {
                    var product =
                        await _productRepository
                            .GetProductByIdAsync(
                                reservationItem.ProductId,
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

                await _orderRepository.AddOrderAsync(order, ct);

                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitTransactionAsync(ct);

                return order.Id;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(ct);
                throw;
            }
        }
        public async Task RemoveItemsFromExpiredReservationsAsync(List<Reservation> expiredReservations,CancellationToken ct)
        {
            foreach (var reservation in expiredReservations)
            {
                await RestoreStockAsync(reservation, ct);

                _reservationRepository.RemoveRangeOfItems(reservation.Items);

                reservation.Status = ReservationStatus.Expired;
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }
        private async Task RestoreStockAsync(Reservation reservation,CancellationToken ct)
        {
            foreach (var item in reservation.Items)
            {
                var product = await _productRepository
                    .GetProductByIdAsync(item.ProductId, ct);

                if (product is null)
                    throw new InvalidOperationException(
                        $"Product with the id {item.ProductId} was not found.");

                product.StockQuantity += item.Quantity;
            }
        }

        private async Task<Reservation> GetActiveReservationAsync(Guid reservationId,CancellationToken ct)
        {
            var reservation =
                await _reservationRepository
                    .GetReservationByIdAsync(reservationId, ct);

            if (reservation is null)
                throw new KeyNotFoundException(
                    $"Reservation with the id {reservationId} was not found.");

            if (!IsReservationActive(reservation))
                throw new InvalidOperationException(
                    "The reservation must be active to perform this operation.");

            return reservation;
        }
    }
}
