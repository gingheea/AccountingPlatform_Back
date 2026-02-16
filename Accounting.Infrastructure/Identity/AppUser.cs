using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Infrastructure.Identity
{
    public class AppUser : IdentityUser<Guid>
    {
        public string? FullName { get; set; }
        public string? TaxId { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}

