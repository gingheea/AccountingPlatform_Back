using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.PricngPackages.DeletePricingPackage
{
    public sealed record DeletePricingPackageCommand(Guid Id) : IRequest;
}
