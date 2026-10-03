using Asal.OrderManagementSystem.Api.Models;

namespace Asal.OrderManagementSystem.Api.Responses
{
    public class ReservationResponse
    {
        private const int ReservationExpirationMinutes = 15;
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeToExpire { get; set; } = string.Empty;

        private ReservationResponse()
        {

        }
        public static ReservationResponse FromModel(Reservation reservation)
        {
            var expiresAt = reservation.CreatedAt.AddMinutes(ReservationExpirationMinutes);

            var remaining = expiresAt - DateTime.UtcNow;

            return new ReservationResponse
            {
                Id = reservation.Id,
                CustomerId = reservation.CustomerId,
                Status = reservation.Status,
                CreatedAt = reservation.CreatedAt,
                TimeToExpire = remaining.TotalMinutes > 0? $"{Math.Ceiling(remaining.TotalMinutes)} min": "Expired"
            };
        }

        public static List<ReservationResponse> FromModels(List<Reservation> reservations)
        {
            return reservations.Select(r => ReservationResponse.FromModel(r)).ToList();
        }
    }
}
