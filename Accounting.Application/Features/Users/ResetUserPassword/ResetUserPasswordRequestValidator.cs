using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.ResetUserPassword
{
    public sealed class ResetUserPasswordRequestValidator
    : AbstractValidator<ResetUserPasswordRequest>
    {
        public ResetUserPasswordRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Не вказано користувача.");

            // Правила складності мають збігатися з політикою Identity
            // (Infrastructure/DependencyInjection.cs): 8 символів, велика,
            // мала й цифра. Спецсимвол там НЕ вимагається — а тут колись
            // вимагався, і адмін не міг задати пароль, який система сама
            // вважає припустимим.
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("Введіть новий пароль.")
                .MinimumLength(8)
                .WithMessage("Пароль має містити щонайменше 8 символів.")
                .MaximumLength(100)
                .Matches("[A-Z]")
                .WithMessage("Пароль має містити щонайменше одну велику літеру.")
                .Matches("[a-z]")
                .WithMessage("Пароль має містити щонайменше одну малу літеру.")
                .Matches("[0-9]")
                .WithMessage("Пароль має містити щонайменше одну цифру.");
        }
    }
}
