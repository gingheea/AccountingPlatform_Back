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

            // The "exactly one of two" rule duplicates the domain invariant on purpose:
            // the validator gives the user a readable 400, while the domain guards
            // against an invalid state if it is ever called around the API.
            RuleFor(x => x)
                .Must(x => (x.ServiceId is not null) ^ (x.PricingPackageId is not null))
                .WithMessage("Select either a service or a pricing package, not both.");
        }
    }
}
