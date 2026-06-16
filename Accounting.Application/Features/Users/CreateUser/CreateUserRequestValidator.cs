using FluentValidation;

namespace Accounting.Application.Features.Users.CreateUser;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    private static readonly string[] AllowedRoles =
    [
        Roles.Admin,
        Roles.User
    ];

    public CreateUserRequestValidator()
    {
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

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.TaxId)
            .MaximumLength(50)
            .WithMessage("Tax ID must not exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.TaxId));

        RuleFor(x => x.Roles)
            .NotNull()
            .WithMessage("Roles are required.")
            .NotEmpty()
            .WithMessage("At least one role is required.");

        RuleForEach(x => x.Roles)
            .NotEmpty()
            .WithMessage("Role cannot be empty.")
            .Must(role => AllowedRoles.Contains(role))
            .WithMessage("Role is not allowed.");

        RuleFor(x => x.Roles)
            .Must(roles => roles.Distinct().Count() == roles.Count)
            .WithMessage("Roles must not contain duplicates.")
            .When(x => x.Roles is not null);
    }
}