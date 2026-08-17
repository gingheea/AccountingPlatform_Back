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
            // Якщо вже підписаний і активний — нічого не робимо й не скаржимось.
            // Повторне натискання не має виглядати як помилка, а повідомлення
            // «ви вже підписані» ще й підказало б стороннім, чия пошта є в базі.

            await _unitOfWork.SaveChangesAsync(ct);

            try
            {
                await _contactClient.AddContactAsync(email, ct);
            }
            catch (Exception ex)
            {
                // Підписка вже збережена в нашій базі — це головне. Якщо Brevo
                // недоступний, людині не варто показувати помилку: вона зробила
                // все правильно. Контакт можна донести в список і пізніше.
                _logger.LogError(ex, "Failed to add {Email} to the newsletter contact list.", email);
            }
        }
    }
}
