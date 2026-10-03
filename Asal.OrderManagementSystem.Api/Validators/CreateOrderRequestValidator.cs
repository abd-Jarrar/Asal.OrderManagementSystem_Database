using Asal.OrderManagementSystem.Api.Requests.OrderRequests;
using FluentValidation;

namespace Asal.OrderManagementSystem.Api.Validators
{
    public class CreateOrderRequestValidator: AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(x => x.Items).NotEmpty().WithMessage("the Order must contain at least one item");
            RuleForEach(r => r.Items).SetValidator(new CreateOrderItemRequestValidator());
        }
    }
}
