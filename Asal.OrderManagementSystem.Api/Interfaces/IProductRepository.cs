using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Responses;

namespace Asal.OrderManagementSystem.Api.Interfaces
{
    public interface IProductRepository
    {
        public Task<Product?> GetProductByIdAsync(Guid productId,CancellationToken ct);

        public Task<Product?> GetProductWithMaximumPriceAsync(CancellationToken ct);

        public Task<Product?> GetProductWithMinimumPriceAsync(CancellationToken ct);

        public Task<Product?> GetProductByNameAsync(string productName,CancellationToken ct);

        public Task<List<Product>> GetAllProductsAsync(CancellationToken ct);

        public Task<bool> DeleteProductByIdAsync(Guid productId, CancellationToken ct);

        public Task<Guid?> CreateProductAsync(string productName, string SKU, decimal productPrice, int? stockQuantity, CancellationToken ct);
        public Task<List<Product>> GetProductWithStockAsync(int stockQuantity,CancellationToken ct);

        public Task<List<Product>>GetActiveProducts(CancellationToken ct);

        public Task<List<Product>>GetProductsSortedByPriceDesc(CancellationToken ct);

        public Task<List<Product>> GetProductsSortedByPriceAsc(CancellationToken ct);

        public Task<List<Product>> GetProductsSortedByName(CancellationToken ct);

        public Task<List<Product>> GetInStockProducts(CancellationToken ct);

        public Task<List<Product>> GetProductsSortedByCreatingDate(CancellationToken ct);

        Task<PagedResult<Product>> GetProductsAsync(int pageNumber,int pageSize,CancellationToken ct);




    }
}
