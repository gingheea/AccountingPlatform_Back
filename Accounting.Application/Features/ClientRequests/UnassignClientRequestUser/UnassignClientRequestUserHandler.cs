using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.UnassignClientRequestUser
{
    public sealed class UnassignClientRequestUserHandler
     : IRequestHandler<UnassignClientRequestUserCommand>
    {
        private readonly IClientRequestRepository _clientRequestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UnassignClientRequestUserHandler(
            IClientRequestRepository clientRequestRepository,
            IUnitOfWork unitOfWork)
        {
            _clientRequestRepository = clientRequestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UnassignClientRequestUserCommand request, CancellationToken ct)
        {
            var clientRequest = await _clientRequestRepository.GetByIdAsync(
                request.ClientRequestId,
                ct);

            if (clientRequest is null)
                throw new NotFoundException("Client request was not found.");

            clientRequest.UnassignUser();

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }  
}
