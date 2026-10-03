using Asal.OrderManagementSystem.Api.Requests.OrderItemRequests;
using FluentValidation;

namespace Asal.OrderManagementSystem.Api.Validators
{
    public class CreateOrderItemRequestValidator: AbstractValidator<CreateOrderItemRequest>
    {
        public CreateOrderItemRequestValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Order item quantity must be positive");
        }
    }
}
