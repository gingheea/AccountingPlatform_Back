using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.PricngPackages.DeactivatePricingPackage
{
    public sealed record DeactivatePricingPackageCommand(Guid Id) : IRequest;
}
