using FluentValidation;

namespace Accounting.Application.Features.ClientDocuments.ChangeStatus
{
    public class Validator : AbstractValidator<ChangeDocumentStatusCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Status)
                .IsInEnum();

            RuleFor(x => x.Note)
                .MaximumLength(2000);
        }
    }
}
