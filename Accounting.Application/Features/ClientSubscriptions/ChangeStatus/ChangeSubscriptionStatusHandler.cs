using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common.Errors;
using Accounting.Domain.Enums;
using Accounting.Domain.Exceptions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientSubscriptions.ChangeStatus
{
    public sealed class ChangeSubscriptionStatusHandler : IRequestHandler<ChangeSubscriptionStatusCommand>
    {
        private readonly IClientSubscriptionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeSubscriptionStatusHandler(
            IClientSubscriptionRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ChangeSubscriptionStatusCommand request, CancellationToken ct)
        {
            var subscription = await _repository.GetByIdAsync(request.Id, ct);

            if (subscription is null)
                throw new NotFoundException($"Subscription with id {request.Id} not found.");

            // Які переходи дозволені — вирішує сама сутність, а не цей обробник.
            Action action = request.Status switch
            {
                SubscriptionStatus.Active => subscription.Resume,
                SubscriptionStatus.Paused => subscription.Pause,
                SubscriptionStatus.Ended => () => subscription.End(),
                _ => throw new DomainException("Unsupported target status.")
            };

            action();

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
