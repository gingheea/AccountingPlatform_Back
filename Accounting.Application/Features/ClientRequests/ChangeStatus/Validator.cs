using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.ChangeStatus
{
    public class Validator : AbstractValidator<ChangeClientRequestStatusCommand>
    {
        public Validator() 
        {
            RuleFor(x => x.Status)
                .IsInEnum();
        }
    }
}
