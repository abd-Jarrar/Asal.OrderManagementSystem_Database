using Asal.OrderManagementSystem.Api.Data;
using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Asal.OrderManagementSystem.Api.Repositories
{
    public class CustomerRepository(AppDbContext _context) : ICustomerRepository
    {
        public async Task<Guid?> CreateCustomerAsync(string customerName, string customerEmail, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentNullException("customer name is empty");

            if (string.IsNullOrWhiteSpace(customerEmail))
                throw new ArgumentNullException("customer email is empty");

            if (await _context.Customers.AnyAsync(c => c.Email == customerEmail, ct))
                return null;
            var customer = new Customer
            {
                Name = customerName,
                Email = customerEmail,
                Id = Guid.NewGuid()
            };
            await _context.AddAsync(customer,ct);
            await _context.SaveChangesAsync(ct);
            return customer.Id;
        }

        public async Task<bool> DeleteCustomerByIdAsync(Guid customerId, CancellationToken ct)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId,ct);
            if (customer is null)
                return false;
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync(ct);
            return true;

        }

        public async Task<List<Customer>> GetAllCustomersAsync(CancellationToken ct)
        {
            return await _context.Customers.ToListAsync(ct);
        }

        public async Task<Customer?> GetCustomerByIdAsync(Guid customerId, CancellationToken ct)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId,ct);
            if (customer is null)
                return null;
            else
                return customer;
        }

        public async Task<List<Customer>> GetCustomersWithOrdersAsync(CancellationToken ct) // using sql
        {
            var customers = await _context.Customers.FromSqlRaw("select * from Customers c where EXISTS (SELECT 1 FROM Orders o WHERE o.CustomerId = c.Id);").ToListAsync(ct);
            return customers;
        }
    }
}
