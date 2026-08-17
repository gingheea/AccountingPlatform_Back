using FluentValidation;

namespace Accounting.Application.Features.Auth.ResetPassword
{
    public class Validator : AbstractValidator<ResetPasswordCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(200);

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Посилання неповне. Скористайтеся ним ще раз з листа.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Введіть новий пароль.")
                .MinimumLength(8).WithMessage("Пароль має містити щонайменше 8 символів.")
                .MaximumLength(100);
        }
    }
}
