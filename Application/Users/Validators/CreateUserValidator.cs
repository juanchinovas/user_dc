using Application.Users.Dtos;
using FluentValidation;

namespace Application.Users.Validators
{
    public class CreateUserValidator : AbstractValidator<CreateUser>
    {
        public CreateUserValidator() {
            RuleFor(user => user.Age)
                .NotNull()
                .WithMessage("The Age is required")
                .LessThan(101)
                .WithMessage("User too old")
                .GreaterThan(20)
                .WithMessage("User too young");

            RuleFor(user => user.FirstName)
                .NotEmpty()
                .NotNull()
                .WithMessage("The FirstName is required")
                .MaximumLength(30)
                .WithMessage("The FirstName is too long");

            RuleFor(user => user.LastName)
                .NotEmpty()
                .NotNull()
                .WithMessage("The LastName is required")
                .MaximumLength(30)
                .WithMessage("The LastName is too long"); ;

            RuleFor(user => user.Country)
                .NotEmpty()
                .NotNull()
                .WithMessage("The Country is required")
                .MaximumLength(30)
                .WithMessage("The Country is too long"); ;

            RuleFor(user => user.City)
                .NotEmpty()
                .NotNull()
                .WithMessage("The City is required")
                .MaximumLength(30)
                .WithMessage("The City is too long"); ;
        }
    }
}
