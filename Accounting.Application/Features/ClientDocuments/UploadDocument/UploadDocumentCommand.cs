using Accounting.Domain.Enums;
using MediatR;
using System;
using System.IO;

namespace Accounting.Application.Features.ClientDocuments.UploadDocument
{
    public sealed record UploadDocumentCommand(
        Guid UserId,
        string Title,
        string FileName,
        string ContentType,
        long SizeBytes,
        Stream Content,
        ClientDocumentCategory Category,
        ClientDocumentDirection Direction,
        string? Note
    ) : IRequest<Guid>;
}
