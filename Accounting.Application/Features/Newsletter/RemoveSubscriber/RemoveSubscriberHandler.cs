using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Newsletter.RemoveSubscriber
{
    public sealed class RemoveSubscriberHandler : IRequestHandler<RemoveSubscriberCommand>
    {
        private readonly INewsletterSubscriberRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveSubscriberHandler(
            INewsletterSubscriberRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RemoveSubscriberCommand request, CancellationToken ct)
        {
            var subscriber = await _repository.Query()
                .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

            if (subscriber is null)
                throw new NotFoundException($"Subscriber with id {request.Id} not found.");

            _repository.Remove(subscriber);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
