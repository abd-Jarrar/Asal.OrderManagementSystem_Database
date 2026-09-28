namespace Asal.OrderManagementSystem.Api.Requests.CustomerRequests
{
    public class CreateCustomerRequest
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string phone { get; set; } = null!; 
    }
}
