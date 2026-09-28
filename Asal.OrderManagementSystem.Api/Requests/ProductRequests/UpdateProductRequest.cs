namespace Asal.OrderManagementSystem.Api.Requests.ProductRequests
{
    public class UpdateProductRequest
    {
        public string? Name { get; set; } = null!;

        public string? SKU { get; set; } = null!;

        public decimal? Price { get; set; }

        public int? stockQuantity { get; set; }

        public bool? IsActive { get; set; }


    }
}
