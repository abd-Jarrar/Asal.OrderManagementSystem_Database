using Asal.OrderManagementSystem.Api.Models;

namespace Asal.OrderManagementSystem.Api.Interfaces
{
    public interface ICustomerRepository
    {
        public Task<Customer?> GetCustomerByIdAsync(Guid customerId,CancellationToken ct);
        public Task<bool> DeleteCustomerByIdAsync(Guid customerId,CancellationToken ct);

        public Task<List<Customer>> GetAllCustomersAsync(CancellationToken ct);

        public Task<Guid?> CreateCustomerAsync(string customerName, string customerEmail,string phone, CancellationToken ct);

        public Task<List<Customer>> GetCustomersWithOrdersAsync(CancellationToken ct);


    }
}
