using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Services.CreateService
{
    public sealed record CreateServiceCommand(
    string Name,
    string? Description,
    decimal Price,
    int SortOrder ) : IRequest<Guid>;

}
