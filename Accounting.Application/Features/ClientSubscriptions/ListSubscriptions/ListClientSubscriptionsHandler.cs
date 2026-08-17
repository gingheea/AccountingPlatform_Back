using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Features.ClientSubscriptions.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientSubscriptions.ListSubscriptions
{
    public sealed class ListClientSubscriptionsHandler
        : IRequestHandler<ListClientSubscriptionsQuery, IReadOnlyList<ClientSubscriptionDto>>
    {
        private readonly IClientSubscriptionRepository _repository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IPricingPackageRepository _pricingPackageRepository;

        public ListClientSubscriptionsHandler(
            IClientSubscriptionRepository repository,
            IServiceRepository serviceRepository,
            IPricingPackageRepository pricingPackageRepository)
        {
            _repository = repository;
            _serviceRepository = serviceRepository;
            _pricingPackageRepository = pricingPackageRepository;
        }

        public async Task<IReadOnlyList<ClientSubscriptionDto>> Handle(
            ListClientSubscriptionsQuery request,
            CancellationToken ct)
        {
            var query = _repository.Query().AsNoTracking();

            if (request.UserId is not null)
                query = query.Where(x => x.UserId == request.UserId.Value);

            if (request.Status is not null)
                query = query.Where(x => x.Status == request.Status.Value);

            var subscriptions = await query
                .OrderByDescending(x => x.StartedAtUtc)
                .ToListAsync(ct);

            if (subscriptions.Count == 0)
                return Array.Empty<ClientSubscriptionDto>();

            // Назви тягнемо двома окремими запитами, а не join'ом: так читабельніше,
            // і на цих обсягах різниці в швидкості немає.
            var serviceIds = subscriptions
                .Where(x => x.ServiceId is not null)
                .Select(x => x.ServiceId!.Value)
                .Distinct()
                .ToList();

            var packageIds = subscriptions
                .Where(x => x.PricingPackageId is not null)
                .Select(x => x.PricingPackageId!.Value)
                .Distinct()
                .ToList();

            var serviceNames = serviceIds.Count == 0
                ? new Dictionary<Guid, string>()
                : await _serviceRepository.Query().AsNoTracking()
                    .Where(x => serviceIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

            var packageNames = packageIds.Count == 0
                ? new Dictionary<Guid, string>()
                : await _pricingPackageRepository.Query().AsNoTracking()
                    .Where(x => packageIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

            return subscriptions
                .Select(x => new ClientSubscriptionDto(
                    x.Id,
                    x.UserId,
                    x.ServiceId,
                    x.PricingPackageId,
                    x.ServiceId is not null && serviceNames.TryGetValue(x.ServiceId.Value, out var sn) ? sn : null,
                    x.PricingPackageId is not null && packageNames.TryGetValue(x.PricingPackageId.Value, out var pn) ? pn : null,
                    x.Status,
                    x.StartedAtUtc,
                    x.EndedAtUtc,
                    x.Note,
                    x.CreatedAtUtc,
                    x.UpdatedAtUtc))
                .ToList();
        }
    }
}
