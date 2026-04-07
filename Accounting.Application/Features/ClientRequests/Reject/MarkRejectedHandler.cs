using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Features.ClientRequests.Complete;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.Reject
{
    public sealed class MarkRejectedHandler : IRequestHandler<MarkRejectedCommand>
    {
        private readonly IClientRequestRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkRejectedHandler(IClientRequestRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(MarkRejectedCommand request, CancellationToken ct)
        {
            var clientRequest = await _repository.GetByIdAsync(request.Id, ct);

            if (clientRequest is null)
            {
                throw new Application.Common.Errors.NotFoundException($"Client Request with id {request.Id} not found.");
            }

            clientRequest.MarkRejected();

            await _unitOfWork.SaveChangesAsync(ct);

        }
    }
}
