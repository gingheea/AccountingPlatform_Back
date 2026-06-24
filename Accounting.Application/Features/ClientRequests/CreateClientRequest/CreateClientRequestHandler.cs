using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Abstractions.Persistence;
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

        public CreateClientRequestHandler(IClientRequestRepository clientRequestRepository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _clientRequestRepository = clientRequestRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
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

            return clientRequest.Id;
        }
    }
}
