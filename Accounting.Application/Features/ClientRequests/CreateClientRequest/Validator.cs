using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.CreateClientRequest
{
    public class Validator : AbstractValidator<CreateClientRequestCommand>
    {
        public Validator() 
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(200);
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(200);
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(50);
            RuleFor(x => x.Message)
                .MaximumLength(4000);
            RuleFor(x => x.RequestType)
                .IsInEnum();
        }
    }
}
