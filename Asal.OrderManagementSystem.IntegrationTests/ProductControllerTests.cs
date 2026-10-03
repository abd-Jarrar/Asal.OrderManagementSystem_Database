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
    public async Task CreateProduct_WithValidDetails_CreatedAt()
    {
        var request = new CreateProductRequest
        {
            Name = "testProduct1",
            SKU = Guid.NewGuid().ToString(),
            Price = 2.00m,
            stockQuantity = 10
        };
        var response = await _httpClient.PostAsJsonAsync("api/products", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidStockQuantity_BadRequest()
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
    public async Task GetProductById_UnKnownProduct_NotFound()
    {
        var unknownProductId = Guid.NewGuid();
        var response = await _httpClient.GetAsync($"api/products/{unknownProductId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_UnKnownProduct_NotFound()
    {
        var unknownProductId = Guid.NewGuid();
        var response = await _httpClient.DeleteAsync($"api/products/{unknownProductId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

    }
    [Fact]
    public async Task DeleteProduct_ProductInOrder_InternalServerError()
    {
        var productId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var response = await _httpClient.DeleteAsync($"api/products/{productId}");
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}
