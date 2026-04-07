using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Features.ClientRequests.Common;
using Accounting.Application.Features.ClientRequests.GetClientRequest;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.GetClientRequestById
{
    internal class GetClientRequestHandler : IRequestHandler<GetClientRequestQuery, ClientRequestDto>
    {
        private readonly IClientRequestRepository _repository;
        private readonly IMapper _mapper;

        public GetClientRequestHandler(IClientRequestRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ClientRequestDto> Handle(GetClientRequestQuery request, CancellationToken ct)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var clientRequest = await _repository.GetByIdAsync(request.Id, ct);

            if (clientRequest == null)
            {
                throw new KeyNotFoundException($"Client request with ID {request.Id} not found.");
            }

            return _mapper.Map<ClientRequestDto>(clientRequest);
        }
    }
}
