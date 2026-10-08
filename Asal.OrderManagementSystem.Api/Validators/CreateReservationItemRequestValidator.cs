using Asal.OrderManagementSystem.Api.Requests.ReservationItemRequests;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace Asal.OrderManagementSystem.Api.Validators
{
    public class CreateReservationItemRequestValidator: AbstractValidator<CreateReservationItemRequest>
    {
        public CreateReservationItemRequestValidator()
        {
            RuleFor(r => r.Quantity).GreaterThan(0).WithMessage("reservation item quantity must be positive");
        }
    }
}
