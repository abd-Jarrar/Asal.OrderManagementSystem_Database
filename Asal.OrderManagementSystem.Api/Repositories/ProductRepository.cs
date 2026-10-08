using Asal.OrderManagementSystem.Api.Data;
using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Responses;
using Microsoft.EntityFrameworkCore;

namespace Asal.OrderManagementSystem.Api.Repositories
{
    public class ProductRepository(AppDbContext _context) : IProductRepository
    {
        public async Task<Guid?> CreateProductAsync(string productName, string SKU, decimal productPrice, int? stockQuantity,CancellationToken ct)
        {
            if (await _context.Products.AnyAsync(p => p.SKU == SKU,ct))
                return null;

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = productName,
                Price = productPrice,
                SKU = SKU,
                StockQuantity = stockQuantity ?? 0,
                IsActive = true,
                CreatedAt= DateTime.UtcNow
            };
            await _context.Products.AddAsync(product, ct);
            await _context.SaveChangesAsync(ct);
            return product.Id;
        }

        public async Task<bool> DeleteProductByIdAsync(Guid productId, CancellationToken ct)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, ct);
            if (product is null)
                return false;
            else
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync(ct);
                return true ;
            }
        }

        public async Task<List<Product>> GetActiveProducts(CancellationToken ct)
        {
            return await _context.Products.Where(p=>p.IsActive).ToListAsync(ct);

        }

        public async Task<List<Product>> GetAllProductsAsync(CancellationToken ct)
        {
            return await _context.Products.ToListAsync(ct);
        }

        public async Task<List<Product>> GetInStockProducts(CancellationToken ct)
        {
            return await _context.Products.Where(x => x.IsActive && x.StockQuantity > 0).ToListAsync(ct);
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken ct)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, ct);
            if (product is null)
                return null;
            return product;
        }

        public async Task<Product?> GetProductByNameAsync(string productName, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return null;
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Name.Equals(productName),ct);
            return product;
        }


        public async Task<PagedResult<Product>> GetProductsAsync(int pageNumber ,int pageSize ,CancellationToken ct )
        {
            var totalCount = await _context.Products
                .CountAsync(ct);

            var products = await _context.Products
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<Product>
            {
                Items = products,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<List<Product>> GetProductsSortedByCreatingDate(CancellationToken ct)
        {
            return await _context.Products.OrderBy(p=>p.CreatedAt).ToListAsync(ct);
        }

        public Task<List<Product>> GetProductsSortedByName(CancellationToken ct)
        {
            return _context.Products.OrderBy(p => p.Name).ToListAsync(ct);
        }

        public Task<List<Product>> GetProductsSortedByPriceAsc(CancellationToken ct)
        {
            return _context.Products.OrderBy(p => p.Price).ToListAsync(ct);
        }

        public Task<List<Product>> GetProductsSortedByPriceDesc(CancellationToken ct)
        {
            return _context.Products.OrderByDescending(p => p.Price).ToListAsync(ct);

        }

        public async Task<Product?> GetProductWithMaximumPriceAsync(CancellationToken ct)
        {
            var product = await _context.Products.OrderByDescending(p => p.Price).FirstOrDefaultAsync(ct);
            return product;
        }

        public async Task<Product?> GetProductWithMinimumPriceAsync(CancellationToken ct)
        {
            var product = await _context.Products.OrderBy(p => p.Price).FirstOrDefaultAsync(ct);
            return product;
        }

        public async Task<List<Product>> GetProductWithStockAsync(int stockQuantity, CancellationToken ct)
        {
            var products = await _context.Products
                .FromSqlInterpolated($"select * from Products where StockQuantity <= {stockQuantity}")
                .ToListAsync(ct);
            return products;
        }

        public void IncreaseStock(Product product, int quantity)
        {
            product?.StockQuantity = quantity;
        }
    }
}
