using Accounting.Application.Features.Users.UpdateUser;
using FluentValidation;

namespace Accounting.Application.Features.Users;

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User id is required.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .MaximumLength(150)
            .WithMessage("Full name must not exceed 150 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email is not valid.")
            .MaximumLength(256)
            .WithMessage("Email must not exceed 256 characters.");

        RuleFor(x => x.TaxId)
            .MaximumLength(50)
            .WithMessage("Tax ID must not exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.TaxId));
    }
}