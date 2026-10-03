using Asal.OrderManagementSystem.Api.Requests.CustomerRequests;
using FluentValidation;

namespace Asal.OrderManagementSystem.Api.Validators
{
    public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
    {
        public CreateCustomerRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Customer name is requied");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Custmoer Email is required");
            RuleFor(x => x.phone).NotEmpty().WithMessage("Customer Phone is required")
                .Length(10, 13).WithMessage("Phone number should be between 10 - 13 number");

        }
    }
}
