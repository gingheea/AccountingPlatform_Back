using Accounting.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.Common
{
    public sealed record ClientRequestDto
        (Guid Id,
        string FullName,
        string Email,
        string? Phone,
        string? Message,
        string? AdminNote,
        Guid? ServiceId,
        Guid? PricingPackageId,
        RequestStatus Status,
        RequestType RequestType,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
        );
}
