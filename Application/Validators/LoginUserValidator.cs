using Application.Request;
using FluentValidation;

namespace Application.Validators
{
    public class LoginUserValidator : AbstractValidator<LoginRequestDTO>
    {

        public LoginUserValidator()
        {

            RuleFor(x => x.Mail)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}

