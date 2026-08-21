using Accounting.Application.Abstractions.Messaging;
using Accounting.Application.Abstractions.Persistence;
using Accounting.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Newsletter.Subscribe
{
    public sealed class SubscribeToNewsletterHandler : IRequestHandler<SubscribeToNewsletterCommand>
    {
        private readonly INewsletterSubscriberRepository _repository;
        private readonly INewsletterContactClient _contactClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SubscribeToNewsletterHandler> _logger;

        public SubscribeToNewsletterHandler(
            INewsletterSubscriberRepository repository,
            INewsletterContactClient contactClient,
            IUnitOfWork unitOfWork,
            ILogger<SubscribeToNewsletterHandler> logger)
        {
            _repository = repository;
            _contactClient = contactClient;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(SubscribeToNewsletterCommand request, CancellationToken ct)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var existing = await _repository.GetByEmailAsync(email, ct);

            if (existing is null)
            {
                await _repository.AddAsync(NewsletterSubscriber.Create(email, request.Source), ct);
            }
            else if (!existing.IsActive)
            {
                existing.Resubscribe(request.Source);
            }
            // Already subscribed and active: do nothing and do not complain.
            // A second click must not look like an error, and an "already subscribed"
            // message would also tell outsiders whose address is in the database.

            await _unitOfWork.SaveChangesAsync(ct);

            try
            {
                await _contactClient.AddContactAsync(email, ct);
            }
            catch (Exception ex)
            {
                // The subscription is already saved on our side, which is what matters.
                // If Brevo is down the person should not see an error: they did everything
                // right. The contact can be pushed to the list later.
                _logger.LogError(ex, "Failed to add {Email} to the newsletter contact list.", email);
            }
        }
    }
}
