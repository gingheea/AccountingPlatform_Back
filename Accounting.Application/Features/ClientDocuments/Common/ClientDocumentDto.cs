using Accounting.Domain.Enums;
using System;

namespace Accounting.Application.Features.ClientDocuments.Common
{
    public sealed record ClientDocumentDto(
        Guid Id,
        Guid UserId,
        string Title,
        string FileName,
        string ContentType,
        long SizeBytes,
        ClientDocumentCategory Category,
        ClientDocumentDirection Direction,
        ClientDocumentStatus Status,
        string? Note,
        DateTimeOffset CreatedAtUtc,
        DateTimeOffset UpdatedAtUtc
    );
}
