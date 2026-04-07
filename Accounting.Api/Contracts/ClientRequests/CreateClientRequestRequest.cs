
using Accounting.Domain.Enums;

namespace Accounting.Api.Contracts.ClientRequests
{
    public sealed record CreateClientRequestRequest(
        string FullName,
        string Email,
        string? PhoneNumber,
        string? Message,
        Guid? ServiceId,
        RequestType RequestType);
}
