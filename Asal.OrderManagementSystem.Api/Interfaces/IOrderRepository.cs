using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Requests.OrderItemRequests;
using Asal.OrderManagementSystem.Api.Requests.OrderRequests;
using Asal.OrderManagementSystem.Api.Responses;

namespace Asal.OrderManagementSystem.Api.Interfaces
{
    public interface IOrderRepository
    {
        public Task<List<Product>> GetTop5SellingProductsAsync(CancellationToken ct);

        public Task<bool> RemoveItemAsync(Guid orderId, Guid productId,CancellationToken ct);
        public Task<List<Order>> GetOrdersCreatedInTheLast30DaysAsync(CancellationToken ct);

        public Task<IEnumerable<MonthlyRevenueDto>> GetMonthlyRevenuesAsync(int year, CancellationToken ct);

        public Task<decimal?> GetCustomerTotalSalesAsync(Guid customerId, CancellationToken ct);

        public Task<Order?> GetOrderByIdAsync(Guid orderId,CancellationToken ct);

        public Task<List<Order>>GetAllOrdersAsync( CancellationToken ct);

        public Task<bool> CancelOrderAsync(Guid orderId, CancellationToken ct);

        public Task UpdateStatus(Guid orderId, OrderStatus newStatus, CancellationToken ct);

        public Task AddItemAsync(Guid orderId, AddOrderItemRequest request,CancellationToken ct);

        Task<Guid> CreateOrderAsync(CreateOrderRequest request,CancellationToken ct);
    }
}
