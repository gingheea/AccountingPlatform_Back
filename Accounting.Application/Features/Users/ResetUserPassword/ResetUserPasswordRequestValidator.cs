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

            // Complexity rules must match the Identity policy
            // (Infrastructure/DependencyInjection.cs): 8 characters, an upper-case,
            // a lower-case letter and a digit. A special character is NOT required
            // there, yet it once was here, and an admin could not set a password the
            // system itself considers valid.
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
