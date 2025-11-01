using EShop.Dto.UserModel;
using FluentValidation;

namespace EShop.Validators
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage("First name is required")
                    .Length(2, 50).WithMessage("First name must be between 2 and 50 characters");

            RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage("Last name is required")
                    .Length(2, 50);

            RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email is required")
                    .EmailAddress().WithMessage("Email format is invalid");

            RuleFor(x => x.Password)
                    .NotEmpty()
                    .MinimumLength(8)
                    .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                    .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                    .Matches("[0-9]").WithMessage("Password must contain at least one number");
        }
    }
}
