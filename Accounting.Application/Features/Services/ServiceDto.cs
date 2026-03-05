using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Services
{
    public sealed record ServiceDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    bool IsActive,
    int SortOrder
);
}
