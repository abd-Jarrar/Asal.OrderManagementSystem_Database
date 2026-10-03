using Asal.OrderManagementSystem.Api.Models;

namespace Asal.OrderManagementSystem.Api.Interfaces
{
    public interface IProductRepository
    {
        public Task<Product?> GetProductByIdAsync(Guid productId,CancellationToken ct);

        public Task<List<Product>> GetAllProductsAsync(CancellationToken ct);

        public Task<bool> DeleteProductByIdAsync(Guid productId, CancellationToken ct);

        public Task<Guid?> CreateProductAsync(string productName, string SKU, decimal productPrice, int? stockQuantity, CancellationToken ct);
        public Task<List<Product>> GetProductWithStockAsync(int stockQuantity,CancellationToken ct);

        public Task<List<Product>>GetActiveProducts(CancellationToken ct);

    }
}
