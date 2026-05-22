using Accounting.Application.Features.PricingPackages.Common;
using Accounting.Application.Features.Services.Common;
using MediatR;

namespace Accounting.Application.Features.PricngPackages.GetPricingPackages
{
    public sealed class GetPricingPackagesQuery() : IRequest<IReadOnlyList<PricingPackageDto>>;
}
