using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.PricngPackages.ActivatePricingPackage
{
    public sealed record ActivatePricingPackageCommand(Guid Id) : IRequest;
}
