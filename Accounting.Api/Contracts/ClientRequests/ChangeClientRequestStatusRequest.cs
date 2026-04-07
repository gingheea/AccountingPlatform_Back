using Accounting.Domain.Enums;

namespace Accounting.Api.Contracts.ClientRequests
{
    public sealed record ChangeClientRequestStatusRequest(RequestStatus Status);
    
}
