using Accounting.Application.Features.ClientDocuments.Common;
using MediatR;
using System;

namespace Accounting.Application.Features.ClientDocuments.GetDownloadUrl
{
    /// <param name="RestrictToUserId">
    /// When set, the document must belong to that user. The portal always passes the
    /// authenticated user id; the admin endpoint passes null.
    /// </param>
    public sealed record GetDocumentDownloadUrlQuery(
        Guid DocumentId,
        Guid? RestrictToUserId
    ) : IRequest<DocumentDownloadDto>;
}
