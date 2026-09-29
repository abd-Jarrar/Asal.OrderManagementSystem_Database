using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Repositories;
using Asal.OrderManagementSystem.Api.Responses;
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
    }
}
