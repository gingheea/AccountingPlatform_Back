using Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Infrastructure.Persistence.Configurations
{
    public sealed class ServiceTagConfiguration : IEntityTypeConfiguration<ServiceTag>
    {
        public void Configure(EntityTypeBuilder<ServiceTag> b)
        {
            b.ToTable("ServiceTags");

            b.HasKey(x => x.Id);

            b.Property(x => x.ServiceId)
                .IsRequired();

            b.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

            b.Property(x => x.SortOrder)
                .IsRequired();

            b.HasIndex(x => new { x.ServiceId, x.Name })
                .IsUnique();
        }
    }
}
