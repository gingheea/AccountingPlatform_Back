using Accounting.Domain.Enums;
using MediatR;
using System;

namespace Accounting.Application.Features.ClientDocuments.ChangeStatus
{
    public sealed record ChangeDocumentStatusCommand(
        Guid Id,
        ClientDocumentStatus Status,
        string? Note
    ) : IRequest;
}
