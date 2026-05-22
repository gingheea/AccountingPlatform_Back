using Accounting.Application.Abstractions.Persistence;
using FluentValidation;

namespace Accounting.Application.Features.PricngPackages.Common
{

    public abstract class BasePricingPackageValidator<T> : AbstractValidator<T> where T : IPricingPackageCommand
    {
        protected BasePricingPackageValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Package name is required.")
                .MaximumLength(200).WithMessage("Package name max length is 200.");

            RuleFor(x => x.Badge)
                .MaximumLength(100).WithMessage("Badge max length is 100.")
                .When(x => !string.IsNullOrWhiteSpace(x.Badge));

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description max length is 1000.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Package price cannot be negative.");

            RuleFor(x => x.PriceLabel)
                .MaximumLength(100).WithMessage("Price label max length is 100.")
                .When(x => !string.IsNullOrWhiteSpace(x.PriceLabel));

            RuleFor(x => x.PeriodLabel)
                .MaximumLength(100).WithMessage("Period label max length is 100.")
                .When(x => !string.IsNullOrWhiteSpace(x.PeriodLabel));

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Sort order cannot be negative.");

            RuleFor(x => x.Features)
                .NotNull().WithMessage("Features collection is required.")
                .Must(features => features == null || features.Count <= 12)
                .WithMessage("Package can have max 12 features.");

            RuleForEach(x => x.Features)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Feature text is required.")
                .MaximumLength(300).WithMessage("Feature text max length is 300.");

            RuleFor(x => x.Features)
                .Must(f => HaveUniqueFeatures(f))
                .WithMessage("Features must be unique.");
        }

        private static bool HaveUniqueFeatures(IReadOnlyCollection<string> features)
        {
            if (features == null) return true; // Захист від NullReferenceException

            var normalizedFeatures = features
                .Where(feature => !string.IsNullOrWhiteSpace(feature))
                .Select(feature => feature.Trim());

            return normalizedFeatures
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count() == normalizedFeatures.Count();
        }
    }
}
