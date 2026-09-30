using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Requests.ProductRequests;
using Asal.OrderManagementSystem.Api.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Asal.OrderManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController(IProductRepository _productRepository,IOrderRepository _orderRepository) : ControllerBase
    {

        
        [HttpGet]
        [Route("{productId:guid}")]
        public async Task<IActionResult> GetProductById(Guid productId, CancellationToken ct)
        {
            var product =await _productRepository.GetProductByIdAsync(productId,ct);
            if (product is null)
                return NotFound(new ProblemDetails
                {
                    Title="product was not found",
                    Detail=$"product with the id {productId} was not found"
                });
            else
            {
                return Ok(ProductResponse.FromModel(product));

            }
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> GetProductByName(string name, CancellationToken ct)
        {
            var product = await _productRepository
                .GetProductByNameAsync(name, ct);

            if (product is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Product was not found",
                    Detail = $"No product with the name '{name}' was found."
                });
            }

            return Ok(ProductResponse.FromModel(product));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10,
    CancellationToken ct = default)
        {
            if (pageNumber <= 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid page number",
                    Detail = "Page number must be greater than zero."
                });
            }

            if (pageSize <= 0 || pageSize > 100)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid page size",
                    Detail = "Page size must be between 1 and 100."
                });
            }

            var result = await _productRepository
                .GetProductsAsync(pageNumber, pageSize, ct);

            if (result.Items.Count == 0)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "There are no products",
                    Detail = "There are no products right now."
                });
            }

            return Ok(new PagedResult<ProductResponse>
            {
                Items = ProductResponse.FromModels(result.Items),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            });
        }

        [HttpGet("max-price")]
        public async Task<IActionResult> GetProductWithMaximumPrice(CancellationToken ct)
        {
            var product = await _productRepository
                .GetProductWithMaximumPriceAsync(ct);

            if (product is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "There are no products",
                    Detail = "There are no products right now."
                });
            }

            return Ok(ProductResponse.FromModel(product));
        }


        [HttpGet("min-price")]
        public async Task<IActionResult> GetProductWithMinimumPrice(CancellationToken ct)
        {
            var product = await _productRepository
                .GetProductWithMinimumPriceAsync(ct);

            if (product is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "There are no products",
                    Detail = "There are no products right now."
                });
            }

            return Ok(ProductResponse.FromModel(product));
        }
        [HttpGet]
        [Route("active-products")]
        public async Task<IActionResult> GetAllActiveProducts(CancellationToken ct)
        {
            var products = await _productRepository.GetActiveProducts(ct);
            if (products.Count == 0)
                return NotFound(new ProblemDetails
                {
                    Title = "there is no products",
                    Detail = "there is no active products right now "
                });
            else
            {
                return Ok(ProductResponse.FromModels(products));
            }
        }

        [HttpDelete]
        [Route("{productId:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid productId, CancellationToken ct)
        {
            var isDeleted = await _productRepository.DeleteProductByIdAsync(productId,ct);

            if (isDeleted)
                return NoContent();
            else
            {
                return NotFound(new ProblemDetails
                {
                    Title="prouct was not found",
                    Detail= $"Product with the id: {productId} was not found"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(CreateProductRequest request,CancellationToken ct)
        {
            try
            {
                Guid? id = await _productRepository.CreateProductAsync(request.Name
                    , request.SKU, request.Price, request.stockQuantity,ct);

                if (id is null)
                    return BadRequest(new ProblemDetails
                    {
                        Title="duplicated sku",
                        Detail="two products can't have the same sku"
                    });

                var product = await _productRepository.GetProductByIdAsync(id.Value,ct);

                var response = ProductResponse.FromModel(product!);

                return CreatedAtAction(
                    nameof(GetProductById),
                    new { productId = product?.Id },
                    response);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = ex.Message,
                });

            }

        }

        [HttpGet]
        [Route("stock-belowOrEqual-5")]
        public async Task<IActionResult> GetProductsWithStockBelow5(CancellationToken ct)
        {
            var products = await _productRepository.GetProductWithStockAsync(5, ct);
            if (products.Count == 0)
                return NotFound(new ProblemDetails
                {
                    Title = "there is no products",
                    Detail = "there is no products with stock less than 5"
                });
            else
                return Ok(products);
        }

        [HttpGet("in-stock")]
        public async Task<IActionResult> GetInStockProducts(CancellationToken ct)
        {
            var products = await _productRepository.GetInStockProducts(ct);

            if (products.Count == 0)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "No products in stock",
                    Detail = "There are currently no products in stock."
                });
            }

            return Ok(ProductResponse.FromModels(products));
        }

        [HttpGet]
        [Route("top-5-selling")]
        public async Task<IActionResult> GetTop5SellingProducts(CancellationToken ct)
        {

            var products = await _orderRepository.GetTop5SellingProductsAsync(ct);

            if (products.Count == 0)
                return NotFound(new ProblemDetails
                {
                    Title = "there is no products",
                    Detail = "there is no products right now "
                });
            return Ok(products);
        }


        [HttpGet("sorted/name")]
        public async Task<IActionResult> GetProductsSortedByName(CancellationToken ct)
        {
            var products = await _productRepository
                .GetProductsSortedByName(ct);

            if (products.Count == 0)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "There are no products",
                    Detail = "There are no products right now."
                });
            }

            return Ok(ProductResponse.FromModels(products));
        }


        [HttpGet("sorted/date")]
        public async Task<IActionResult> GetProductsSortedByCreatedDate(CancellationToken ct)
        {
            var products = await _productRepository
                .GetProductsSortedByCreatingDate(ct);

            if (products.Count == 0)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "There are no products",
                    Detail = "There are no products right now."
                });
            }

            return Ok(ProductResponse.FromModels(products));
        }


        [HttpGet("sorted/price/asc")]
        public async Task<IActionResult> GetProductsSortedByPriceAsc(CancellationToken ct)
        {
            var products = await _productRepository
                .GetProductsSortedByPriceAsc(ct);

            if (products.Count == 0)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "There are no products",
                    Detail = "There are no products right now."
                });
            }

            return Ok(ProductResponse.FromModels(products));
        }

        [HttpGet("sorted/price/desc")]
        public async Task<IActionResult> GetProductsSortedByPriceDesc(CancellationToken ct)
        {
            var products = await _productRepository
                .GetProductsSortedByPriceDesc(ct);

            if (products.Count == 0)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "There are no products",
                    Detail = "There are no products right now."
                });
            }

            return Ok(ProductResponse.FromModels(products));
        }
    }
}
