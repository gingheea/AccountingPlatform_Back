using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Services.UpdateService
{
    internal class Validator : AbstractValidator<UpdateServiceCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name max length is 200.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description max length is 2000.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).WithMessage("SortOrder cannot be negative.");
        }
    }
}
