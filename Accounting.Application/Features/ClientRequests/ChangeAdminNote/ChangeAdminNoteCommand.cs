using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.ChangeAdminNote
{
    public sealed record ChangeAdminNoteCommand(Guid Id, string adminNote) : IRequest;
}
