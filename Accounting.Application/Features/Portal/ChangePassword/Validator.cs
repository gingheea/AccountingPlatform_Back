using FluentValidation;

namespace Accounting.Application.Features.Portal.ChangePassword
{
    public class Validator : AbstractValidator<ChangeOwnPasswordCommand>
    {
        public Validator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Введіть поточний пароль.");

            // Complexity rules live in the Identity options and are enforced on save.
            // Only the bare minimum here, so obviously empty input is not sent onward.
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Введіть новий пароль.")
                .MinimumLength(8).WithMessage("Пароль має містити щонайменше 8 символів.")
                .MaximumLength(100);

            RuleFor(x => x.NewPassword)
                .NotEqual(x => x.CurrentPassword)
                .WithMessage("Новий пароль має відрізнятися від поточного.");
        }
    }
}
