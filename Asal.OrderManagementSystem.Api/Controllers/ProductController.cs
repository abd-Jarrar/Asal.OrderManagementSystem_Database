using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Requests.ProductRequests;
using Asal.OrderManagementSystem.Api.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Asal.OrderManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {

        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
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
        public async Task<IActionResult> GetAllProducts(CancellationToken ct)
        {
            var products = await _productRepository.GetAllProductsAsync(ct);
            if (products.Count==0)
                return NotFound(new ProblemDetails
                {
                    Title="there is no products",
                    Detail= "there are no products right now "
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
    }
}
