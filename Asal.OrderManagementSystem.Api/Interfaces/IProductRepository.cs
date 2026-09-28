using Asal.OrderManagementSystem.Api.Models;

namespace Asal.OrderManagementSystem.Api.Interfaces
{
    public interface IProductRepository
    {
        public Product? GetProductById(Guid productId);

        public List<Product> GetAllProducts();

        public bool DeleteProductById(Guid productId);

        public Guid? CreateProduct(string productName, string? SKU, decimal productPrice, int? stockQuantity);
        public List<Product> GetProductWithStock(int stockQuantity);

    }
}
