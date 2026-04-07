using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common.Errors;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.ChangeStatus
{
    public sealed class ChangeClientRequestStatusHandler : IRequestHandler<ChangeClientRequestStatusCommand>
    {
        private readonly IClientRequestRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeClientRequestStatusHandler(IClientRequestRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ChangeClientRequestStatusCommand request, CancellationToken ct)
        {
            var clientRequest = await _repository.GetByIdAsync(request.Id, ct);

            if (clientRequest is null)
                throw new Application.Common.Errors.NotFoundException($"Client Request with id {request.Id} not found.");

            Action action = request.Status switch
            {
                RequestStatus.InProgress => clientRequest.MarkInProgress,
                RequestStatus.WaitingForClient => clientRequest.MarkWaitingForClient,
                _ => throw new DomainException("Unsupported target status.")
            };

            action();

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
