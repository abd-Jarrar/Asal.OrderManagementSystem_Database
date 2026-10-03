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
            var timeToExpire = 
                reservation.Status == ReservationStatus.Active
        ? GetRemainingTime(reservation.CreatedAt)
        : reservation.Status.ToString();

            return new ReservationResponse
            {
                Id = reservation.Id,
                CustomerId = reservation.CustomerId,
                Status = reservation.Status,
                CreatedAt = reservation.CreatedAt,
                TimeToExpire = timeToExpire
            };
        }

        public static List<ReservationResponse> FromModels(List<Reservation> reservations)
        {
            return reservations.Select(r => ReservationResponse.FromModel(r)).ToList();
        }

        private static string GetRemainingTime(DateTime createdAt)
        {
            var expiresAt = createdAt.AddMinutes(ReservationExpirationMinutes);
            var remaining = expiresAt - DateTime.UtcNow;

            return remaining.TotalMinutes > 0
                ? $"{Math.Ceiling(remaining.TotalMinutes)} min"
                : "Expired";
        }
    }
}
