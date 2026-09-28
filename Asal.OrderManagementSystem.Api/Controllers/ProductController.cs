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
        public IActionResult GetProductById(Guid productId)
        {
            var product = _productRepository.GetProductById(productId);
            if (product is null)
                return NotFound();
            else
            {
                return Ok(ProductResponse.FromModel(product));

            }
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var products = _productRepository.GetAllProducts();
            if (products is null)
                return NotFound("there are no products right now ");
            else
            {
                return Ok(ProductResponse.FromModels(products));
            }
        }

        [HttpDelete]
        [Route("{productId:guid}")]
        public IActionResult DeleteProduct(Guid productId)
        {
            var isDeleted = _productRepository.DeleteProductById(productId);

            if (isDeleted)
                return NoContent();
            else
            {
                return NotFound($"Product with the id: {productId} was not found");
            }
        }

        [HttpPost]
        public IActionResult AddProduct(CreateProductRequest request)
        {
            try
            {
                Guid? id = _productRepository.CreateProduct(request.Name
                    , request.SKU, request.Price, request.stockQuantity);

                if (id is null)
                    return BadRequest();

                var product = _productRepository.GetProductById(id.Value);

                var response = ProductResponse.FromModel(product!);

                return CreatedAtAction(
                    nameof(GetProductById),
                    new { productId = product?.Id },
                    response);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);

            }

        }

        [HttpPut]
        [Route("{productId:guid}")]
        public IActionResult UpdateProduct(Guid productId, UpdateProductRequest request)
        {
            try
            {
                var updateCustomer = _productRepository.UpdateProduct(productId, request.Name, request.SKU
                    , request.Price, request.stockQuantity, request.IsActive);
                if (updateCustomer)
                    return NoContent();
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

    }
}
