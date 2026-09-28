namespace Asal.OrderManagementSystem.Api.Requests.ProductRequests
{
    public class CreateProductRequest
    {

        public string Name { get; set; } = null!;

        public string? SKU { get; set; }

        public decimal Price { get; set; }

        public int? stockQuantity { get; set; }

    }
}
