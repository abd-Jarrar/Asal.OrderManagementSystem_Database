using Asal.OrderManagementSystem.Api.Data;
using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Requests.OrderItemRequests;
using Asal.OrderManagementSystem.Api.Requests.OrderRequests;
using Asal.OrderManagementSystem.Api.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace Asal.OrderManagementSystem.Api.Repositories
{
    public class OrderRepository(AppDbContext _context,ILogger<OrderRepository> _logger) : IOrderRepository
    {
        public async Task<bool> CancelOrderAsync(Guid orderId, CancellationToken ct)
        {
            var order = await GetOrderByIdAsync(orderId, ct);
            if(order is null)
                throw new ArgumentNullException($"order with the id {orderId} was not found ");
            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("you can't cancel a cancelled order");
            if (order.Status == OrderStatus.Completed)
                throw new InvalidOperationException("you can't cancel a completed order");
            foreach (var orderItem in order.OrderItems)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == orderItem.ProductId, ct);
                if (product is null || !product.IsActive)
                    throw new InvalidOperationException("Failed to cancel the order because it contains unavailable products");
                product.StockQuantity += orderItem.Quantity;
            }
            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync(ct);
            _logger.LogInformation("the order with id: {OrderId} was cancelled", orderId);
            return true;
        }

        public async Task<bool> RemoveItemAsync(Guid orderId,Guid productId, CancellationToken ct)
        {
            var order = await GetOrderByIdAsync(orderId, ct);
            if (order is null)
                throw new KeyNotFoundException($"Order with the id {orderId} was not found.");
            var orderItem = order.OrderItems.FirstOrDefault(oi=>oi.ProductId==productId);
            if (orderItem is null)
                throw new KeyNotFoundException($"Product with the id {productId} was not found in the order.");
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == orderItem.ProductId,ct);
            if (product is null || !product.IsActive)
                throw new InvalidOperationException("Failed to remove the item because the product is unavailable.");
            product.StockQuantity += orderItem.Quantity;
            order.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<List<Order>> GetAllOrdersAsync(CancellationToken ct)
        {
           
            return await _context.Orders.Include(o=>o.OrderItems).ToListAsync(ct);
        }

        public async Task<decimal> GetCustomerTotalSalesAsync(Guid customerId,CancellationToken ct)
        {
            var customer= await _context.Customers.FirstOrDefaultAsync(x => x.Id == customerId,ct);
            if(customer is null)
                throw new ArgumentNullException($"there is no customer with the id {customerId}");

            var orders = await _context.Orders.FromSqlInterpolated($"select * from Orders where CustomerId={customerId} ").ToListAsync(ct);
            return orders.Sum(o => o.TotalAmount);

        }

        public async Task<IEnumerable<MonthlyRevenueDto>> GetMonthlyRevenuesAsync(int year,CancellationToken ct)
        {
            var orders = await _context.Orders.FromSqlInterpolated($"SELECT * FROM Orders WHERE YEAR(CreatedAt) = {year}").ToListAsync(ct);
            
            return orders
    .GroupBy(o => o.CreatedAt.Month).Select(g => MonthlyRevenueDto.
    FromDateAndRevenue(
        new DateTime(year, g.Key, 1),
        g.Sum(o => o.TotalAmount))
    );
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId, CancellationToken ct)
        {
            return await _context.Orders.Include(o=>o.OrderItems).FirstOrDefaultAsync(o=>o.Id== orderId,ct);
        }

        public async Task<List<Order>> GetOrdersCreatedInTheLast30DaysAsync(CancellationToken ct)
        {
            return await _context.Orders.FromSqlRaw("SELECT * FROM Orders WHERE CreatedAt >= DATEADD(DAY, -30, GETUTCDATE())").Include(o => o.OrderItems).ToListAsync(ct);

        }

        public async Task<List<Product>> GetTop5SellingProductsAsync(CancellationToken ct)
        {
            return await _context.Products.FromSqlRaw("SELECT TOP 5 p.* FROM Products p INNER JOIN OrderItems oi ON p.Id = oi.ProductId GROUP BY p.Id, p.Name, p.Price, p.SKU, p.StockQuantity, p.IsActive, p.CreatedAt ORDER BY SUM(oi.Quantity) DESC").ToListAsync(ct);
        }

        public async Task UpdateStatus(Guid orderId, OrderStatus newStatus, CancellationToken ct)
        {
            var order = await GetOrderByIdAsync(orderId, ct);
            if (order is null)
                throw new KeyNotFoundException($"Order with the id {orderId} was not found.");
            if (newStatus == OrderStatus.Shipped && order.Status == OrderStatus.Pending)
                order.Status = newStatus;
            else if (newStatus == OrderStatus.Completed && order.Status == OrderStatus.Shipped)
                order.Status = newStatus;
            else
            {
                throw new InvalidOperationException("failed to update the status");
            }
            await _context.SaveChangesAsync(ct);

        }

        public async Task AddItemAsync(Guid orderId, AddOrderItemRequest request ,CancellationToken ct)
        {
            var order = await GetOrderByIdAsync(orderId, ct);
            if (order is null)
                throw new KeyNotFoundException($"Order with the id {orderId} was not found.");
            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException(
                    "Items can only be added to pending orders.");

            var product=await _context.Products.FirstOrDefaultAsync(p=>p.Id==request.ProductId,ct);

            if (product is null)
                throw new KeyNotFoundException($"product with the id {request.ProductId} was not found.");
            if (!product.IsActive)
                throw new InvalidOperationException(
                    "Cannot add an inactive product to an order.");
            if (product.StockQuantity < request.Quantity)
            {
                _logger.LogWarning("There is no enough stock for product {product.Id}", product.Id);

                throw new InvalidOperationException($"there's no enough quantity in the stock");
            }

            product.StockQuantity-= request.Quantity;
            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId= orderId,
                ProductId = product.Id,
                Quantity = request.Quantity,
                UnitPrice = product.Price
            };
            order.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync(ct);
            
        }

        public async Task<Guid> CreateOrderAsync(CreateOrderRequest request,CancellationToken ct)
        {
                await using var transaction =await _context.Database.BeginTransactionAsync(ct);
            
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == request.CustomerId, ct);

                if (customer is null)
                    throw new KeyNotFoundException($"Customer with the id {request.CustomerId} was not found.");

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = request.CustomerId,
                    Status = OrderStatus.Pending,
                    OrderItems = new List<OrderItem>()
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
                    _logger.LogWarning("There is no enough stock for product {product.Id}", product.Id);
                    throw new InvalidOperationException($"There is no enough stock for product {product.Id}.");
                    
                }
                    
                    product.StockQuantity -= item.Quantity;
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };

                    order.OrderItems.Add(orderItem);
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(ct);
            _logger.LogInformation("order with the id: {OrderId} was created", order.Id);
                await transaction.CommitAsync(ct);
                return order.Id;
            
        }
    }
}
