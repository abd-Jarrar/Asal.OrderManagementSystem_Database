using Asal.OrderManagementSystem.Api.Models;

namespace Asal.OrderManagementSystem.Api.Responses
{
    public class CustomerResponse
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set;  } = null!;
        private CustomerResponse()
        {

        }
        public static CustomerResponse FromModel(Customer customer)
        {
            if (customer is null)
                throw new ArgumentNullException(nameof(customer), "cannot create a response from null customer");
            var customerResponse = new CustomerResponse()
            {
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,

            };
            return customerResponse;
        }
        public static List<CustomerResponse> FromModels(IEnumerable<Customer> customers)
        {
            return customers.Select(c => CustomerResponse.FromModel(c)).ToList();
        }
    }
}
