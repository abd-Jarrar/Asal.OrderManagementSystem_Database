using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Models;
using Asal.OrderManagementSystem.Api.Repositories;
using Asal.OrderManagementSystem.Api.Requests.OrderRequests;
using Asal.OrderManagementSystem.Api.Requests.ReservationItemRequests;
using Asal.OrderManagementSystem.Api.Requests.ReservationRequests;
using Asal.OrderManagementSystem.Api.Responses;
using Asal.OrderManagementSystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Asal.OrderManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationController(ReservationService _reservationService):ControllerBase
    {

        [HttpGet]
        [Route("{reservationId:guid}")]
        public async Task<IActionResult> GetReservationById(Guid reservationId, CancellationToken ct)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(reservationId, ct);
            if (reservation is null)
                return NotFound(new ProblemDetails
                {
                    Title = "no reservation was found",
                    Detail = $"reservation with the id {reservationId} was not found"
                });
            return Ok(ReservationResponse.FromModel(reservation));
        }
        [HttpGet]
        public async Task<IActionResult> GetReservations(CancellationToken ct)
        {
            var reservations = await _reservationService.GetAllReservationsAsync(ct);
            if (reservations.Count() == 0)
                return NotFound(new ProblemDetails
                {
                    Title = "no reservations found",
                    Detail = "there is no reservations right now"
                });
            return Ok(ReservationResponse.FromModels(reservations));
        }

        [HttpGet]
        [Route("expired-reservations")]
        public async Task<IActionResult> GetExpiredReservations(CancellationToken ct)
        {
            var reservations = await _reservationService.GetExpiredReservationsAsync(ct);
            if (reservations.Count() == 0)
                return NotFound(new ProblemDetails
                {
                    Title = "no reservations found",
                    Detail = "there is no expired reservations right now"
                });
            return Ok(ReservationResponse.FromModels(reservations));
        }


        [HttpPost]
        public async Task<IActionResult> CreateReservation(CreateReservationRequest request, CancellationToken ct)
        {
            var reservationId = await _reservationService.CreateReservationAsync(request, ct);

            return Created(
                $"/api/reservations/{reservationId}",
                new { reservationId });
        }

        [HttpPost]
        [Route("cancel/{reservationId:guid}")]
        public async Task<IActionResult> CancelReservation(Guid reservationId, CancellationToken ct)
        {
            await _reservationService.CancelReservationAsync(reservationId, ct);
            return NoContent();
        }


        [HttpPost]
        [Route("convert/{reservationId:guid}")]
        public async Task<IActionResult> ConvertReservationToOrder(Guid reservationId,CancellationToken ct)
        {
            var orderId = await _reservationService.ConvertReservationToOrderAsync(reservationId, ct);

            return Created(
                $"/api/orders/{orderId}",
                new { orderId });
        }
    }
}
