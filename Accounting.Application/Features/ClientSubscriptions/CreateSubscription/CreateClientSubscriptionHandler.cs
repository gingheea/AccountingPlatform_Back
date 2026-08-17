using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common.Errors;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientSubscriptions.CreateSubscription
{
    public sealed class CreateClientSubscriptionHandler
        : IRequestHandler<CreateClientSubscriptionCommand, System.Guid>
    {
        private readonly IClientSubscriptionRepository _repository;
        private readonly IUserManagementService _userManagementService;
        private readonly IServiceRepository _serviceRepository;
        private readonly IPricingPackageRepository _pricingPackageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateClientSubscriptionHandler(
            IClientSubscriptionRepository repository,
            IUserManagementService userManagementService,
            IServiceRepository serviceRepository,
            IPricingPackageRepository pricingPackageRepository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _userManagementService = userManagementService;
            _serviceRepository = serviceRepository;
            _pricingPackageRepository = pricingPackageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<System.Guid> Handle(CreateClientSubscriptionCommand request, CancellationToken ct)
        {
            if (!await _userManagementService.ExistsAsync(request.UserId, ct))
                throw new NotFoundException($"User with id {request.UserId} not found.");

            if (request.ServiceId is not null &&
                await _serviceRepository.GetByIdAsync(request.ServiceId.Value, ct) is null)
                throw new NotFoundException($"Service with id {request.ServiceId} not found.");

            if (request.PricingPackageId is not null &&
                await _pricingPackageRepository.GetByIdAsync(request.PricingPackageId.Value, ct) is null)
                throw new NotFoundException($"Pricing package with id {request.PricingPackageId} not found.");

            // Двічі вести того самого клієнта за тим самим пакетом безглуздо:
            // це або помилка оператора, або дубль після повторного натискання.
            var alreadyActive = await _repository.Query()
                .AsNoTracking()
                .AnyAsync(
                    x => x.UserId == request.UserId
                         && x.Status != SubscriptionStatus.Ended
                         && x.ServiceId == request.ServiceId
                         && x.PricingPackageId == request.PricingPackageId,
                    ct);

            if (alreadyActive)
                throw new ConflictException("This client already has an active subscription for the selection.");

            var subscription = ClientSubscription.Create(
                request.UserId,
                request.ServiceId,
                request.PricingPackageId,
                request.StartedAtUtc,
                request.Note);

            await _repository.AddAsync(subscription, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return subscription.Id;
        }
    }
}
