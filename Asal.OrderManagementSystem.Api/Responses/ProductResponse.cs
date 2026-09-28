using Asal.OrderManagementSystem.Api.Models;

namespace Asal.OrderManagementSystem.Api.Responses
{
    public class ProductResponse
    {
        public string Name { get; set; } = null!;

        public string SKU { get; set; } = null!;

        public decimal Price { get; set; }

        public int stockQuantity { get; set; }

        public bool IsActive { get; set; }
        private ProductResponse()
        {

        }
        public static ProductResponse FromModel(Product product)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product), "cannot create a response from null product");
            var response = new ProductResponse()
            {
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price,
                stockQuantity = product.StockQuantity,
                IsActive=product.IsActive


            };
            return response;
        }
        public static List<ProductResponse> FromModels(IEnumerable<Product> products)
        {
            return products.Select(p => ProductResponse.FromModel(p)).ToList();
        }
    }
}
