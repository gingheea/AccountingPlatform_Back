using Accounting.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.CreateClientRequest
{
    public sealed record CreateClientRequestCommand
        (string FullName,
        string Email,
        string? PhoneNumber,
        string? Message,
        Guid? ServiceId,
        Guid? PricingPackageId,
        RequestType RequestType) : IRequest<Guid>
    {
    }
}
