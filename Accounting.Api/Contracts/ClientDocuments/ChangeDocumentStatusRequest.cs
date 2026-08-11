using Accounting.Domain.Enums;

namespace Accounting.Api.Contracts.ClientDocuments
{
    public sealed record ChangeDocumentStatusRequest(
        ClientDocumentStatus Status,
        string? Note
    );
}
