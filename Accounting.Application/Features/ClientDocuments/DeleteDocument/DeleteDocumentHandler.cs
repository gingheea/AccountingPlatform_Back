using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Abstractions.Storage;
using Accounting.Application.Common.Errors;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientDocuments.DeleteDocument
{
    public sealed class DeleteDocumentHandler : IRequestHandler<DeleteDocumentCommand>
    {
        private readonly IClientDocumentRepository _repository;
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDocumentHandler(
            IClientDocumentRepository repository,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _fileStorage = fileStorage;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteDocumentCommand request, CancellationToken ct)
        {
            var document = await _repository.GetByIdAsync(request.Id, ct);

            if (document is null)
                throw new NotFoundException($"Document with id {request.Id} not found.");

            var storageKey = document.StorageKey;

            _repository.Remove(document);
            await _unitOfWork.SaveChangesAsync(ct);

            // The row is gone; a blob left behind would only be dead weight, never a leak.
            await _fileStorage.DeleteAsync(storageKey, ct);
        }
    }
}
