using Asal.OrderManagementSystem.Api.Data;
using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Models;
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

        public async Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken ct)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, ct);
            if (product is null)
                return null;
            return product;
        }

        public async Task<List<Product>> GetProductWithStockAsync(int stockQuantity, CancellationToken ct)
        {
            var products = await _context.Products
                .FromSqlInterpolated($"select * from Products where StockQuantity <= {stockQuantity}")
                .ToListAsync(ct);
            return products;
        }
    }
}
