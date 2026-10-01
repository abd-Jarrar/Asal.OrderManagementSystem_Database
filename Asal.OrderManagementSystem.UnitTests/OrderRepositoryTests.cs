using Asal.OrderManagementSystem.Api.Data;
using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Repositories;
using Asal.OrderManagementSystem.Api.Requests.OrderItemRequests;
using Asal.OrderManagementSystem.Api.Requests.OrderRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;

namespace Asal.OrderManagementSystem.UnitTests;

public class OrderRepositoryTests : IDisposable
{
    private readonly OrderRepository _orderRepository;
    private readonly AppDbContext _context;

    Guid activeCustomerId =Guid.Parse("00000000-0000-0000-0000-000000000000");

    Guid ValidProductId =Guid.Parse("00000000-0000-0000-0000-000000000000");
    Guid ValidProductId2 = Guid.Parse("55555555-5555-5555-5555-555555555555");
    Guid InvalidCustomerId = Guid.Parse("99999999-9999-9999-9999-999999999999");
    Guid InValidProductId = Guid.Parse("99999999-9999-9999-9999-999999999999");

    Guid InvalidOrderId = Guid.Parse("00000000-0000-0000-0000-000000000000");

    Guid validOrderId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

    
    Guid CanclledOrderId = Guid.Parse("02c47f99-0777-4acd-a784-17013de6804c");
    Guid CompletedOrderId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    Guid InActiveProductId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    CancellationToken ct = CancellationToken.None;
    public OrderRepositoryTests()
    {
        var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

        var connectionString =
            configuration.GetConnectionString("testDb");

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connectionString).Options;

        _context = new AppDbContext(options);

        _orderRepository = new OrderRepository(_context,NullLogger<OrderRepository>.Instance);
    }


    [Fact]
    public async Task CancelOrderAsync_WithValidOrder_ReturnTrue()
    {
        // Arrange

        var createOrderRequest = new CreateOrderRequest
        {
            CustomerId = activeCustomerId,
            Items = new List<CreateOrderItemRequest>
        {
            new CreateOrderItemRequest
            {
                ProductId = ValidProductId,
                Quantity = 1
            }
        }
        };

        // Act
        Guid orderId = await _orderRepository.CreateOrderAsync(createOrderRequest,ct);
        await _orderRepository.CancelOrderAsync(orderId, ct);

        // Assert
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);

        Assert.Equal(OrderStatus.Cancelled, order?.Status);
    }

    [Fact]
    public async Task CheckStockRestorationCancelOrderAsync_WithValidOrder_ReturnTrue()
    {
        // Arrange
        var quantityBefore = (await _context.Products.FirstOrDefaultAsync(p => p.Id == ValidProductId, ct))?.StockQuantity;
        var createOrderRequest = new CreateOrderRequest
        {
            CustomerId = activeCustomerId,
            Items = new List<CreateOrderItemRequest>
        {
            new CreateOrderItemRequest
            {
                ProductId = ValidProductId,
                Quantity = 1
            }
        }
        };

        // Act
        Guid orderId = await _orderRepository.CreateOrderAsync(createOrderRequest, ct);
        await _orderRepository.CancelOrderAsync(orderId, ct);

        // Assert
        var quantityAfter = (await _context.Products.FirstOrDefaultAsync(p => p.Id == ValidProductId, ct))?.StockQuantity;
        Assert.Equal(quantityAfter, quantityBefore);

    }


   
    [Fact]
    public async Task CancelOrderAsync_WithCompletedOrder_ThrowsInvalidOperationException()
    {
        // Arrange

        var order = await _orderRepository.GetOrderByIdAsync(CompletedOrderId, ct);


        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
             () => _orderRepository.CancelOrderAsync(CompletedOrderId, ct));

        Assert.Contains("completed", exception.Message);

    }

    [Fact]
    public async Task CreateOrder_WithInvalidCustomer_ThrowsKeyNotFoundException()
    {
        // Arrange
        var createOrderRequest = new CreateOrderRequest
        {
            CustomerId = InvalidCustomerId,
            Items = new List<CreateOrderItemRequest>
        {
            new CreateOrderItemRequest
            {
                ProductId = ValidProductId,
                Quantity = 1
            }
        }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _orderRepository.CreateOrderAsync(createOrderRequest, ct));

        Assert.Contains("Customer", exception.Message);
        Assert.Contains(InvalidCustomerId.ToString(), exception.Message);
    }



    [Fact]
    public async Task CreateOrder_WithInvalidProduct_ThrowsKeyNotFoundException()
    {
        // Arrange
        var createOrderRequest = new CreateOrderRequest
        {
            CustomerId = activeCustomerId,
            Items = new List<CreateOrderItemRequest>
        {
            new CreateOrderItemRequest
            {
                ProductId = InValidProductId,
                Quantity = 1
            }
        }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _orderRepository.CreateOrderAsync(createOrderRequest, ct));

        Assert.Contains("Product", exception.Message);
        Assert.Contains(InValidProductId.ToString(), exception.Message);
    }


    [Fact]
    public async Task CreateOrder_InSufficientStock_ThrowsInvalidOperationException()
    {
        // Arrange

        var createOrderRequest = new CreateOrderRequest
        {
            CustomerId = activeCustomerId,
            Items = new List<CreateOrderItemRequest>
        {
            new CreateOrderItemRequest
            {
                ProductId = ValidProductId,
                Quantity = int.MaxValue
            }
        }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _orderRepository.CreateOrderAsync(createOrderRequest, ct));

        Assert.Contains("stock", exception.Message);
    }


    [Fact]
    public async Task CreateOrder_InActiveProduct_ThrowsInvalidOperationException()
    {
        // Arrange

        var createOrderRequest = new CreateOrderRequest
        {
            CustomerId = activeCustomerId,
            Items = new List<CreateOrderItemRequest>
        {
            new CreateOrderItemRequest
            {
                ProductId = InActiveProductId,
                Quantity = int.MaxValue
            }
        }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _orderRepository.CreateOrderAsync(createOrderRequest, ct));

        Assert.Contains("inactive", exception.Message);
    }


    [Fact]
    public async Task CreateOrder_With2ValidProducts_CreatesOrder()
    {
        // Arrange

        var createOrderRequest = new CreateOrderRequest
        {
            CustomerId = activeCustomerId,
            Items = new List<CreateOrderItemRequest>
        {
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
        }
        };

        // Act
        Guid orderId = await _orderRepository.CreateOrderAsync(createOrderRequest, ct);

        try
        {
            // Assert
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId, ct);

            Assert.NotNull(order);
            Assert.Equal(activeCustomerId, order.CustomerId);
            Assert.Equal(OrderStatus.Pending, order.Status);

            Assert.Equal(2, order.OrderItems.Count());
        }
        finally
        {
            await _orderRepository.CancelOrderAsync(orderId, ct);
        }
    }


    [Fact]
    public async Task CreateOrder_With1ValidProduct_CreatesOrder()
    {
        // Arrange

        var createOrderRequest = new CreateOrderRequest
        {
            CustomerId = activeCustomerId,
            Items = new List<CreateOrderItemRequest>
        {
            new CreateOrderItemRequest
            {
                ProductId = ValidProductId,
                Quantity = 1
            }
        }
        };

        // Act
        Guid orderId = await _orderRepository.CreateOrderAsync(createOrderRequest, ct);

        try
        {
            // Assert
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId, ct);

            Assert.NotNull(order);
            Assert.Equal(activeCustomerId, order.CustomerId);
            Assert.Equal(OrderStatus.Pending, order.Status);


            var orderItem = order.OrderItems.First();

            Assert.Equal(ValidProductId, orderItem.ProductId);
            Assert.Equal(1, orderItem.Quantity);
        }
        finally
        {
            await _orderRepository.CancelOrderAsync(orderId, ct);
        }
    }


    [Fact]
    public async Task RemoveItem_InvalidOrderId_ThrowsKeyNotFoundException()
    {
        //Act & Assert
        var excepion = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        _orderRepository.RemoveItemAsync(InvalidOrderId, ValidProductId, ct));

        Assert.Contains("not found", excepion.Message);

    }

    [Fact]
    public async Task RemoveItem_InvalidProductId_ThrowsKeyNotFoundException()
    {
        //Act & Assert
        var excepion = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        _orderRepository.RemoveItemAsync(validOrderId, ValidProductId, ct));

        Assert.Contains("not found",excepion.Message);
    }

    [Fact]
    public async Task CancelOrderAsync_WithCancelledOrder_ThrowsInvalidOperationException()
    {
        // Arrange

        var order = await _orderRepository.GetOrderByIdAsync(CanclledOrderId, ct);


        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
             () => _orderRepository.CancelOrderAsync(CanclledOrderId, ct));

        Assert.Contains("cancelled", exception.Message);

    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
