using Accounting.Application.Common;
using Accounting.Application.Features.ClientDocuments.Common;
using Accounting.Domain.Enums;
using MediatR;
using System;

namespace Accounting.Application.Features.ClientDocuments.ListDocuments
{
    /// <param name="UserId">
    /// Restricts the result to a single client. The portal always sets it from the
    /// authenticated user, so a client can never read someone else's documents.
    /// </param>
    public sealed record ListDocumentsQuery(
        Guid? UserId,
        ClientDocumentCategory? Category,
        ClientDocumentDirection? Direction,
        ClientDocumentStatus? Status,
        int Page,
        int PageSize
    ) : IRequest<PagedResult<ClientDocumentDto>>;
}
