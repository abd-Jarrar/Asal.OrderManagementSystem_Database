using Asal.OrderManagementSystem.Api.Requests.ProductRequests;
using System.Net;
using System.Net.Http.Json;

namespace Asal.OrderManagementSystem.IntegrationTests;

public class ProductControllerTests : IClassFixture<CustomWebApplicationFactory>
{
   

    private readonly HttpClient _httpClient;
    public ProductControllerTests(CustomWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }
    [Fact]
    public async Task CreateProduct_WithValidDetails_ReturnsCreated()
    {
        //arrange
        var request = new CreateProductRequest
        {
            Name = "testProduct1",
            SKU = Guid.NewGuid().ToString(),
            Price = 2.00m,
            stockQuantity = 10
        };
        //act
        var response = await _httpClient.PostAsJsonAsync("api/products", request);
        //assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidStockQuantity_ReturnsBadRequest()
    {
        //arrange
        var request = new CreateProductRequest()
        {
            Name = "test product2",
            SKU = Guid.NewGuid().ToString(),
            Price = 2.99m,
            stockQuantity = -10
        };
        //act 
        var response = await _httpClient.PostAsJsonAsync("api/products", request);
        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

    }

    [Fact]
    public async Task GetProductById_UnknownProduct_ReturnsNotFound()
    {
        //arrange
        var unknownProductId = Guid.NewGuid();
        //act
        var response = await _httpClient.GetAsync($"api/products/{unknownProductId}");
        //assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_UnknownProduct_ReturnsNotFound()
    {
        //arrange
        var unknownProductId = Guid.NewGuid();
        //act
        var response = await _httpClient.DeleteAsync($"api/products/{unknownProductId}");
        //assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    [Fact]
    public async Task DeleteProduct_ProductInOrder_ReturnsInternalServerError()
    {
        //arrange
        var productId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        //act
        var response = await _httpClient.DeleteAsync($"api/products/{productId}");
        //assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}
