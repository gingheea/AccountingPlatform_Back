using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Features.Services.CreateService;
using Accounting.Domain.Entities;
using MediatR;

namespace Accounting.Application.Features.Services.Create;

public sealed class Handler : IRequestHandler<CreateServiceCommand, Guid>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public Handler(IServiceRepository serviceRepository, IUnitOfWork unitOfWork)
    {
        _serviceRepository = serviceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateServiceCommand request, CancellationToken ct)
    {
        var service = Service.Create(
            request.Name,
            request.Price,
            request.Description,
            request.SortOrder
        );

        await _serviceRepository.AddAsync(service, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return service.Id;
    }
}