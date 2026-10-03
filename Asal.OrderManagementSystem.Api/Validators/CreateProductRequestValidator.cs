using Asal.OrderManagementSystem.Api.Requests.ProductRequests;
using FluentValidation;

namespace Asal.OrderManagementSystem.Api.Validators
{
    public class CreateProductRequestValidator :AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required");
            RuleFor(x => x.SKU).NotEmpty().WithMessage("Product SKU is required");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Product Price should be positive");
            RuleFor(x => x.stockQuantity).GreaterThanOrEqualTo(0).WithMessage("Stock quantity can't be negative");

        }
    }
}
