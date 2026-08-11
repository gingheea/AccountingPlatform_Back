using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common.Errors;
using Accounting.Domain.Enums;
using Accounting.Domain.Exceptions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientDocuments.ChangeStatus
{
    public sealed class ChangeDocumentStatusHandler : IRequestHandler<ChangeDocumentStatusCommand>
    {
        private readonly IClientDocumentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeDocumentStatusHandler(
            IClientDocumentRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ChangeDocumentStatusCommand request, CancellationToken ct)
        {
            var document = await _repository.GetByIdAsync(request.Id, ct);

            if (document is null)
                throw new NotFoundException($"Document with id {request.Id} not found.");

            Action action = request.Status switch
            {
                ClientDocumentStatus.InReview => document.MarkInReview,
                ClientDocumentStatus.Approved => document.Approve,
                ClientDocumentStatus.Rejected => () => document.Reject(request.Note),
                ClientDocumentStatus.Archived => document.Archive,
                _ => throw new DomainException("Unsupported target status.")
            };

            action();

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
