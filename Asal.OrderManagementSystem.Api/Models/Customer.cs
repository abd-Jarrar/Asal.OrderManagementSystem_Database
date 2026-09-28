namespace Asal.OrderManagementSystem.Api.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
