using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Repositories;
using Asal.OrderManagementSystem.Api.Requests.OrderRequests;
using Asal.OrderManagementSystem.Api.Requests.ReservationItemRequests;
using Asal.OrderManagementSystem.Api.Requests.ReservationRequests;
using Asal.OrderManagementSystem.Api.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Asal.OrderManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationController(IReservationRepository _reservationRepository):ControllerBase
    {

        [HttpGet]
        [Route("{reservationId:guid}")]
        public async Task<IActionResult> GetReservationById(Guid reservationId, CancellationToken ct)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId, ct);
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
            var reservations = await _reservationRepository.GetAllReservations(ct);
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
            var reservations = await _reservationRepository.GetExpiredReservationsAsync(ct);
            if (reservations.Count() == 0)
                return NotFound(new ProblemDetails
                {
                    Title = "no reservations found",
                    Detail = "there is no reservations right now"
                });
            return Ok(ReservationResponse.FromModels(reservations));
        }


        [HttpPost]
        public async Task<IActionResult> CreateReservation(CreateReservationRequest request, CancellationToken ct)
        {
            var reservationId = await _reservationRepository.CreateReservationAsync(request, ct);

            return Created(
                $"/api/reservations/{reservationId}",
                new { reservationId });
        }

        [HttpPost]
        [Route("cancel/{reservationId:guid}")]
        public async Task<IActionResult> CancelOrder(Guid reservationId, CancellationToken ct)
        {
            bool isDeleted = await _reservationRepository.CancelReservationAsync(reservationId, ct);
            if (isDeleted)
                return NoContent();
            else
            {
                return NotFound(new ProblemDetails
                {
                    Title = "cancelling failed",
                    Detail = "order with the id {reservationId} was not found"
                });
            }

        }


        [HttpPost]
        [Route("convert/{reservationId:guid}")]
        public async Task<IActionResult> ConvertReservationToOrder(Guid reservationId,CancellationToken ct)
        {
            var converted = await _reservationRepository.ConvertReservationToOrder(reservationId, ct);

            if (!converted)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Reservation not found",
                    Detail = $"Reservation with the id {reservationId} was not found."
                });
            }

            return NoContent();
        }
    }
}
