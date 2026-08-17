using FluentValidation;

namespace Accounting.Application.Features.ClientSubscriptions.CreateSubscription
{
    public class Validator : AbstractValidator<CreateClientSubscriptionCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.Note)
                .MaximumLength(2000);

            // Правило «рівно одне з двох» дублює доменний інваріант навмисно:
            // валідатор дає користувачу зрозумілу помилку 400, а домен
            // страхує від некоректного стану, якщо його викличуть повз API.
            RuleFor(x => x)
                .Must(x => (x.ServiceId is not null) ^ (x.PricingPackageId is not null))
                .WithMessage("Select either a service or a pricing package, not both.");
        }
    }
}
