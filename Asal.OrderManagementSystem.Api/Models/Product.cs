namespace Asal.OrderManagementSystem.Api.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public string SKU { get; set; } = null!;


        public decimal Price { get; set; }

        public int stockQuantity { get; set; }

        public bool IsActive { get; set; }
    }
}
