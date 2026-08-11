using Accounting.Application.Features.ClientDocuments.Common;
using FluentValidation;

namespace Accounting.Application.Features.ClientDocuments.UploadDocument
{
    public class Validator : AbstractValidator<UploadDocumentCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.FileName)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.SizeBytes)
                .GreaterThan(0)
                .LessThanOrEqualTo(AllowedDocumentContentTypes.MaxSizeBytes)
                .WithMessage($"File is larger than {AllowedDocumentContentTypes.MaxSizeBytes / (1024 * 1024)} MB.");

            RuleFor(x => x.ContentType)
                .NotEmpty()
                .Must(AllowedDocumentContentTypes.Values.Contains)
                .WithMessage("This file type is not allowed.");

            RuleFor(x => x.Category)
                .IsInEnum();

            RuleFor(x => x.Direction)
                .IsInEnum();

            RuleFor(x => x.Note)
                .MaximumLength(2000);
        }
    }
}
