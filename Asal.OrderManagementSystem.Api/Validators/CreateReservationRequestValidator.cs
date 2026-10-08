using Asal.OrderManagementSystem.Api.Requests.ReservationRequests;
using FluentValidation;

namespace Asal.OrderManagementSystem.Api.Validators
{
    public class CreateReservationRequestValidator: AbstractValidator<CreateReservationRequest>
    {
        public CreateReservationRequestValidator()
        {
            RuleFor(r => r.Items).NotEmpty().WithMessage("the Reservation must contain at least one item");
            RuleForEach(r => r.Items).SetValidator(new CreateReservationItemRequestValidator());
        }
    }
}
