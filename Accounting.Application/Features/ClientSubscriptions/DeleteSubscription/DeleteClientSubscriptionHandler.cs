using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common.Errors;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientSubscriptions.DeleteSubscription
{
    public sealed class DeleteClientSubscriptionHandler : IRequestHandler<DeleteClientSubscriptionCommand>
    {
        private readonly IClientSubscriptionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteClientSubscriptionHandler(
            IClientSubscriptionRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteClientSubscriptionCommand request, CancellationToken ct)
        {
            var subscription = await _repository.GetByIdAsync(request.Id, ct);

            if (subscription is null)
                throw new NotFoundException($"Subscription with id {request.Id} not found.");

            _repository.Remove(subscription);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
