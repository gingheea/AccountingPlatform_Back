using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.AssignClientRequestToUser
{
    public sealed class AssignClientRequestToUserHandler
     : IRequestHandler<AssignClientRequestToUserCommand>
    {
        private readonly IClientRequestRepository _clientRequestRepository;
        private readonly IUserManagementService _userManagementService;
        private readonly IUnitOfWork _unitOfWork;

        public AssignClientRequestToUserHandler(
            IClientRequestRepository clientRequestRepository,
            IUserManagementService userManagementService,
            IUnitOfWork unitOfWork)
        {
            _clientRequestRepository = clientRequestRepository;
            _userManagementService = userManagementService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AssignClientRequestToUserCommand request, CancellationToken ct)
        {
            var clientRequest = await _clientRequestRepository.GetByIdAsync(
                request.ClientRequestId,
                ct);

            if (clientRequest is null)
                throw new NotFoundException("Client request was not found.");

            var userExists = await _userManagementService.ExistsAsync(request.UserId, ct);

            if (!userExists)
                throw new NotFoundException("User was not found.");

            clientRequest.AssignUser(request.UserId);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
