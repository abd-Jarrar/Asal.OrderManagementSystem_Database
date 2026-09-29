using Asal.OrderManagementSystem.Api.Data;
using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Responses;
using Microsoft.EntityFrameworkCore;

namespace Asal.OrderManagementSystem.Api.Repositories
{
    public class OrderRepository(AppDbContext _context) : IOrderRepository
    {

        public async Task<List<Order>> GetAllOrdersAsync(CancellationToken ct)
        {
           
            return await _context.Orders.ToListAsync(ct);
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
            return await _context.Orders.FirstOrDefaultAsync(o=>o.Id== orderId,ct);
        }

        public async Task<List<Order>> GetOrdersCreatedInTheLast30DaysAsync(CancellationToken ct)
        {
            return await _context.Orders.FromSqlRaw("SELECT * FROM Orders WHERE CreatedAt >= DATEADD(DAY, -30, GETUTCDATE())").Include(o => o.OrderItems).ToListAsync(ct);

        }

        public async Task<List<Product>> GetTop5SellingProductsAsync(CancellationToken ct)
        {
            return await _context.Products.FromSqlRaw("SELECT TOP 5 p.* FROM Products p INNER JOIN OrderItems oi ON p.Id = oi.ProductId GROUP BY p.Id, p.Name, p.Price, p.SKU, p.StockQuantity, p.IsActive, p.CreatedAt ORDER BY SUM(oi.Quantity) DESC").ToListAsync(ct);
        }
    }
}
