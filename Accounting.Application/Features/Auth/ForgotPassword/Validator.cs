using FluentValidation;

namespace Accounting.Application.Features.Auth.ForgotPassword
{
    public class Validator : AbstractValidator<ForgotPasswordCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Вкажіть адресу пошти.")
                .EmailAddress().WithMessage("Схоже, адреса введена з помилкою.")
                .MaximumLength(200);
        }
    }
}
