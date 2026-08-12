using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common.Errors;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.DeleteClientRequest
{
    public sealed class DeleteClientRequestHandler : IRequestHandler<DeleteClientRequestCommand>
    {
        private readonly IClientRequestRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteClientRequestHandler(
            IClientRequestRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteClientRequestCommand request, CancellationToken ct)
        {
            var clientRequest = await _repository.GetByIdAsync(request.Id, ct);

            if (clientRequest is null)
                throw new NotFoundException($"Client Request with id {request.Id} not found.");

            _repository.Remove(clientRequest);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
