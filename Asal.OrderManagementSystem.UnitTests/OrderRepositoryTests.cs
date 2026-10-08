using Asal.OrderManagementSystem.Api.Data;
using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Repositories;
using Asal.OrderManagementSystem.Api.Requests.OrderItemRequests;
using Asal.OrderManagementSystem.Api.Requests.OrderRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
namespace Asal.OrderManagementSystem.UnitTests;

public class OrderRepositoryTests : IDisposable
{
    private readonly OrderRepository _orderRepository;
    private readonly AppDbContext _context;

    private static readonly Guid ActiveCustomerId = Guid.Parse("00000000-0000-0000-0000-000000000000");

    private static readonly Guid ValidProductId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    private static readonly Guid ValidProductId2 = Guid.Parse("55555555-5555-5555-5555-555555555555");


    private static readonly Guid ValidOrderId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");


    private static readonly Guid CancelledOrderId = Guid.Parse("02c47f99-0777-4acd-a784-17013de6804c");
    private static readonly Guid CompletedOrderId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    private static readonly Guid InActiveProductId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly CancellationToken CT = CancellationToken.None;
    public OrderRepositoryTests()
    {
        var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

        var connectionString =
            configuration.GetConnectionString("testDb");

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connectionString).Options;

        _context = new AppDbContext(options);

        _orderRepository = new OrderRepository(_context, NullLogger<OrderRepository>.Instance);
    }


    [Fact]
    public async Task CancelOrderAsync_WithValidOrder_ReturnTrue()
    {
        // Arrange

        var request = CreateOrderRequestWithOneProduct(ActiveCustomerId, ValidProductId, 1);

        // Act
        Guid orderId = await _orderRepository.CreateOrderAsync(request, CT);
        await _orderRepository.CancelOrderAsync(orderId, CT);

        // Assert
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId, CT);

        Assert.Equal(OrderStatus.Cancelled, order?.Status);
    }

    [Fact]
    public async Task CheckStockRestorationCancelOrderAsync_WithValidOrder_ReturnTrue()
    {
        // Arrange
        var quantityBefore = (await _context.Products.FirstOrDefaultAsync(p => p.Id == ValidProductId, CT))?.StockQuantity;


        var request = CreateOrderRequestWithOneProduct(ActiveCustomerId, ValidProductId, 1);

        // Act
        Guid orderId = await _orderRepository.CreateOrderAsync(request, CT);
        await _orderRepository.CancelOrderAsync(orderId, CT);

        // Assert
        var quantityAfter = (await _context.Products.FirstOrDefaultAsync(p => p.Id == ValidProductId, CT))?.StockQuantity;
        Assert.Equal(quantityAfter, quantityBefore);

    }



    [Fact]
    public async Task CancelOrderAsync_WithCompletedOrder_ThrowsInvalidOperationException()
    {


        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
             () => _orderRepository.CancelOrderAsync(CompletedOrderId, CT));

        Assert.Contains("completed", exception.Message);

    }

    [Fact]
    public async Task CreateOrder_WithInvalidCustomer_ThrowsKeyNotFoundException()
    {
        // Arrange
        var newId = Guid.NewGuid();

        var request = CreateOrderRequestWithOneProduct(newId, ValidProductId, 1);
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _orderRepository.CreateOrderAsync(request, CT));

        Assert.Contains("Customer", exception.Message);
        Assert.Contains(newId.ToString(), exception.Message);
    }



    [Fact]
    public async Task CreateOrder_WithInvalidProduct_ThrowsKeyNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var request = CreateOrderRequestWithOneProduct(ActiveCustomerId,productId,1);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _orderRepository.CreateOrderAsync(request, CT));

        Assert.Contains("Product", exception.Message);
        Assert.Contains(productId.ToString(), exception.Message);
    }


    [Fact]
    public async Task CreateOrder_InSufficientStock_ThrowsInvalidOperationException()
    {
        // Arrange


        var request = CreateOrderRequestWithOneProduct(ActiveCustomerId, ValidProductId, int.MaxValue);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _orderRepository.CreateOrderAsync(request, CT));

        Assert.Contains("stock", exception.Message);
    }


    [Fact]
    public async Task CreateOrder_InActiveProduct_ThrowsInvalidOperationException()
    {
        // Arrange

        var request = CreateOrderRequestWithOneProduct(ActiveCustomerId, InActiveProductId, 1);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _orderRepository.CreateOrderAsync(request, CT));

        Assert.Contains("inactive", exception.Message);
    }


    [Fact]
    public async Task CreateOrderAsync_WithTwoValidProduct_CreatesOrder()
    {
        // Arrange
        var request = CreateOrderRequestWithMultipleProducts(ActiveCustomerId, [
            new CreateOrderItemRequest
            {
                ProductId = ValidProductId,
                Quantity = 1
            },
            new CreateOrderItemRequest
            {
                ProductId = ValidProductId2,
                Quantity = 1
            }
        ]);



        // Act
        Guid orderId = await _orderRepository.CreateOrderAsync(request, CT);

        try
        {
            // Assert
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId, CT);

            Assert.NotNull(order);
            Assert.Equal(ActiveCustomerId, order.CustomerId);
            Assert.Equal(OrderStatus.Pending, order.Status);

            Assert.Equal(2, order.OrderItems.Count());
        }
        finally
        {
            await _orderRepository.CancelOrderAsync(orderId, CT);
        }
    }


    [Fact]
    public async Task CreateOrder_With1ValidProduct_CreatesOrder()
    {
        // Arrange

        var request = CreateOrderRequestWithOneProduct(ActiveCustomerId, ValidProductId, 1);



        // Act
        Guid orderId = await _orderRepository.CreateOrderAsync(request, CT);

        try
        {
            // Assert
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId, CT);

            Assert.NotNull(order);
            Assert.Equal(ActiveCustomerId, order.CustomerId);
            Assert.Equal(OrderStatus.Pending, order.Status);


            var orderItem = order.OrderItems.First();

            Assert.Equal(ValidProductId, orderItem.ProductId);
            Assert.Equal(1, orderItem.Quantity);
        }
        finally
        {
            await _orderRepository.CancelOrderAsync(orderId, CT);
        }
    }


    [Fact]
    public async Task RemoveItem_InvalidOrderId_ThrowsKeyNotFoundException()
    {
        var orderId = Guid.NewGuid();
        //Act & Assert
        var excepion = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        _orderRepository.RemoveItemAsync(orderId, ValidProductId, CT));

        Assert.Contains("not found", excepion.Message);

    }

    [Fact]
    public async Task RemoveItem_InvalidProductId_ThrowsKeyNotFoundException()
    {
        //Act & Assert
        var excepion = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        _orderRepository.RemoveItemAsync(ValidOrderId, ValidProductId, CT));

        Assert.Contains("not found", excepion.Message);
    }

    [Fact]
    public async Task CancelOrderAsync_WithCancelledOrder_ThrowsInvalidOperationException()
    {


        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
             () => _orderRepository.CancelOrderAsync(CancelledOrderId, CT));

        Assert.Contains("cancelled", exception.Message);

    }

    public void Dispose()
    {
        _context.Dispose();
    }
    private static CreateOrderRequest CreateOrderRequestWithMultipleProducts(Guid customerId, CreateOrderItemRequest[] items)
    {
        return new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = items.ToList()
        };
    }

    private static CreateOrderRequest CreateOrderRequestWithOneProduct(Guid customerId, Guid productId,int quantity)
    {
        return new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = [
                new CreateOrderItemRequest
                {
                    ProductId = productId,
                    Quantity = quantity
                }
            ]
        };
    }
}
