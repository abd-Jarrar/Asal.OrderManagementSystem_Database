using Asal.OrderManagementSystem.Api.Models;

namespace Asal.OrderManagementSystem.Api.Interfaces
{
    public interface ICustomerRepository
    {
        public Customer? GetCustomerById(Guid customerId);
        public bool DeleteCustomerById(Guid customerId);

        public List<Customer> GetAllCustomers();

        public Guid? CreateCustomer(string customerName, string customerEmail);

        public List<Customer> GetCustomersWithOrders();

        public decimal GetCustomerTotalSales(Guid customerId);

    }
}
