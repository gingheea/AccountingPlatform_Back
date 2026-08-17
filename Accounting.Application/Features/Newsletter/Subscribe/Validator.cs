using FluentValidation;

namespace Accounting.Application.Features.Newsletter.Subscribe
{
    public class Validator : AbstractValidator<SubscribeToNewsletterCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Вкажіть адресу пошти.")
                .EmailAddress().WithMessage("Схоже, адреса введена з помилкою.")
                .MaximumLength(200);

            RuleFor(x => x.Source)
                .MaximumLength(50);
        }
    }
}
