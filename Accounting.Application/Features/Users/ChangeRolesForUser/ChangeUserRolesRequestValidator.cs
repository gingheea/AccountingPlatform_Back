
using FluentValidation;

namespace Accounting.Application.Features.Users.ChangeRolesForUser;

public sealed class ChangeUserRolesRequestValidator
    : AbstractValidator<ChangeUserRolesRequest>
{
    private static readonly string[] AllowedRoles =
    [
        Roles.Admin,
        Roles.User
    ];

    public ChangeUserRolesRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User id is required.");

        RuleFor(x => x.Roles)
            .NotNull()
            .WithMessage("Roles are required.")
            .Must(roles => roles.Any())
            .WithMessage("At least one role is required.");

        RuleForEach(x => x.Roles)
            .NotEmpty()
            .WithMessage("Role cannot be empty.")
            .Must(role => AllowedRoles.Contains(role))
            .WithMessage("Role is not allowed.");

        RuleFor(x => x.Roles)
            .Must(roles => roles.Distinct().Count() == roles.Count())
            .WithMessage("Roles must not contain duplicates.")
            .When(x => x.Roles is not null);
    }
}