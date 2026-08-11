using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Abstractions.Storage;
using Accounting.Application.Common.Errors;
using Accounting.Application.Features.ClientDocuments.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientDocuments.GetDownloadUrl
{
    public sealed class GetDocumentDownloadUrlHandler
        : IRequestHandler<GetDocumentDownloadUrlQuery, DocumentDownloadDto>
    {
        private static readonly TimeSpan UrlLifetime = TimeSpan.FromMinutes(5);

        private readonly IClientDocumentRepository _repository;
        private readonly IFileStorage _fileStorage;

        public GetDocumentDownloadUrlHandler(
            IClientDocumentRepository repository,
            IFileStorage fileStorage)
        {
            _repository = repository;
            _fileStorage = fileStorage;
        }

        public async Task<DocumentDownloadDto> Handle(
            GetDocumentDownloadUrlQuery request,
            CancellationToken ct)
        {
            var document = await _repository.GetByIdAsync(request.DocumentId, ct);

            if (document is null)
                throw new NotFoundException($"Document with id {request.DocumentId} not found.");

            if (request.RestrictToUserId is not null && document.UserId != request.RestrictToUserId.Value)
                throw new ForbiddenException("This document belongs to another client.");

            var url = await _fileStorage.GetDownloadUrlAsync(
                document.StorageKey,
                document.FileName,
                UrlLifetime,
                ct);

            return new DocumentDownloadDto(
                url,
                document.FileName,
                DateTimeOffset.UtcNow.Add(UrlLifetime));
        }
    }
}
