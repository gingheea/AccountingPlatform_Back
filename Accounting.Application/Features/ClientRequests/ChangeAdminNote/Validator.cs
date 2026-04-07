using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.ChangeAdminNote
{
    public class Validator : AbstractValidator<ChangeAdminNoteCommand>
    {
        public Validator() 
        {
            RuleFor(x => x.adminNote)
                .NotEmpty()
                .MaximumLength(2000);
        }
    }
}
