using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Features.ClientRequests.Events;
using Accounting.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.CreateClientRequest
{
    public sealed class CreateClientRequestHandler : IRequestHandler<CreateClientRequestCommand, Guid>
    {
        private readonly IClientRequestRepository _clientRequestRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;

        public CreateClientRequestHandler(IClientRequestRepository clientRequestRepository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IPublisher publisher)
        {
            _clientRequestRepository = clientRequestRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }
        public async Task<Guid> Handle(CreateClientRequestCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;

            var clientRequest = ClientRequest.Create(
                request.FullName,
                request.Email,
                request.PhoneNumber,
                request.Message,
                request.ServiceId,
                request.PricingPackageId,
                request.RequestType,
                currentUserId
            );

            await _clientRequestRepository.AddAsync(clientRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Only after a successful save: otherwise an email could go out about
            // a request that is not in the database.
            await _publisher.Publish(
                new ClientRequestCreatedNotification(
                    clientRequest.Id,
                    clientRequest.FullName,
                    clientRequest.Email,
                    clientRequest.Phone,
                    clientRequest.Message,
                    clientRequest.RequestType,
                    clientRequest.ServiceId,
                    clientRequest.PricingPackageId,
                    clientRequest.CreatedAtUtc),
                cancellationToken);

            return clientRequest.Id;
        }
    }
}
