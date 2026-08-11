using MediatR;
using System;

namespace Accounting.Application.Features.ClientDocuments.DeleteDocument
{
    public sealed record DeleteDocumentCommand(Guid Id) : IRequest;
}
