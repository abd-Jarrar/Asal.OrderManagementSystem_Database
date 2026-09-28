using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Responses;

namespace Asal.OrderManagementSystem.Api.Interfaces
{
    public interface IOrderRepository
    {
        public List<Product> GetTop5SellingProducts();

        public List<Order> GetOrdersCreatedInTheLast30Days();

        public IEnumerable<MonthlyRevenueDto> GetMonthlyRevenues();


    }
}
