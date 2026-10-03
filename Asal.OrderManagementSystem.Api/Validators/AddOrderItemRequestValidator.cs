using Asal.OrderManagementSystem.Api.Requests.OrderItemRequests;
using FluentValidation;

namespace Asal.OrderManagementSystem.Api.Validators
{
    public class AddOrderItemRequestValidator: AbstractValidator<AddOrderItemRequest>
    {
        public AddOrderItemRequestValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Order Item quantity must be positive");
        }
    }
}
