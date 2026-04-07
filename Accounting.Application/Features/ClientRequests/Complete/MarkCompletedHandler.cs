using Accounting.Application.Abstractions.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.Complete
{
    public sealed class MarkCompletedHandler : IRequestHandler<MarkCompletedCommand>
    {
        private readonly IClientRequestRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkCompletedHandler(IClientRequestRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(MarkCompletedCommand request, CancellationToken ct)
        {
            var clientRequest = await _repository.GetByIdAsync(request.Id, ct);

            if (clientRequest is null) 
            {
              throw new Application.Common.Errors.NotFoundException($"Client Request with id {request.Id} not found.");
            }

            clientRequest.MarkCompleted();

            await _unitOfWork.SaveChangesAsync(ct);
          
        }
    }
}
