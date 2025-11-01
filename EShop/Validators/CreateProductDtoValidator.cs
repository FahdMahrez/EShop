using EShop.Dto.ProductModel;
using FluentValidation;

namespace EShop.Validators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .Length(2, 100);

            RuleFor(x => x.CostPrice)
                .GreaterThan(0).WithMessage("Price must be greater than zero");

            RuleFor(x => x.SellingPrice)
                .GreaterThan(0).WithMessage("Price must be greater than zero");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category ID is required");
        }
    }
}
