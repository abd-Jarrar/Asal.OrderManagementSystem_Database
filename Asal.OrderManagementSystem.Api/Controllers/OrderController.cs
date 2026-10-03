using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Repositories;
using Asal.OrderManagementSystem.Api.Requests.OrderItemRequests;
using Asal.OrderManagementSystem.Api.Requests.OrderRequests;
using Asal.OrderManagementSystem.Api.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Asal.OrderManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController(IOrderRepository _orderRepository) : ControllerBase
    {
        [HttpGet]
        [Route("last-30-days-orders")]
        public async Task<IActionResult> GetLast30DaysOrders(CancellationToken ct)
        {
            var orders = await _orderRepository.GetOrdersCreatedInTheLast30DaysAsync(ct);
            if (orders.Count == 0)
                return NotFound(new ProblemDetails
                {
                    Title = "no orders",
                    Detail = "there is no orders right now "
                });
            else
            {
                return Ok(OrderResponse.FromModels(orders));
            }
        }

        [HttpGet]
        [Route("monthly-revenues-{year:int}")]
        public async Task<IActionResult> GetMontlyRevenues(int year,CancellationToken ct)
        {
            var revenues = await _orderRepository.GetMonthlyRevenuesAsync(year, ct);
            if (revenues.Count() == 0)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "no revenues",
                    Detail = $"there is no orders in this year {year}"
                });
            }
            return Ok(revenues);
        }

        [HttpGet]
        [Route("{orderId:guid}")]
        public async Task<IActionResult> GetOrderById(Guid orderId,CancellationToken ct)
        {
            var order=await _orderRepository.GetOrderByIdAsync(orderId, ct);
            if (order is null)
                return NotFound(new ProblemDetails
                {
                    Title = "no order was found",
                    Detail = $"order with the id {orderId} was not found"
                });
            return Ok(OrderResponse.FromModel(order));
        }


        [HttpGet]
        public async Task<IActionResult> GetOrders(CancellationToken ct)
        {
            var orders=await _orderRepository.GetAllOrdersAsync(ct);
            if (orders.Count() == 0)
                return NotFound(new ProblemDetails
                {
                    Title="no orders found",
                    Detail="there is no orders right now"
                });
            return Ok(OrderResponse.FromModels(orders));
        }

        [HttpPost]
        [Route("cancel/{orderId:guid}")]
        public async Task<IActionResult> CancelOrder(Guid orderId, CancellationToken ct)
        {

            bool isDeleted = await _orderRepository.CancelOrderAsync(orderId, ct);
            if (isDeleted)
                return NoContent();
            else
            {
                return NotFound(new ProblemDetails
                {
                    Title = "cancelling failed",
                    Detail = "order with the id {orderId} was not found"
                });
            }

        }

        [HttpDelete]
        [Route("{orderId:guid}/items/{itemId:guid}")]
        public async Task<IActionResult> RemoveItem(Guid orderId,Guid itemId,CancellationToken ct)
        {
            await _orderRepository.RemoveItemAsync(orderId, itemId, ct);

            return NoContent();
        }

        [HttpPut]
        [Route("{orderId}/{status}")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId,OrderStatus status,CancellationToken ct)
        {
            await _orderRepository.UpdateStatus(orderId, status, ct);
            return NoContent();
        }

        [HttpPost("{orderId:guid}/items")]
        public async Task<IActionResult> AddOrderItem(Guid orderId,[FromBody] AddOrderItemRequest request,CancellationToken ct)
        {
            await _orderRepository.AddItemAsync(orderId, request, ct);

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request,CancellationToken ct)
        {
            var orderId = await _orderRepository.CreateOrderAsync(request, ct);

            return Created(
                $"/api/orders/{orderId}",
                new { orderId });
        }
    }
}
