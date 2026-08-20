using Accounting.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.Persistence
{
    public interface ITestimonialRepository
    {
        Task<Testimonial?> GetByIdAsync(Guid id, CancellationToken ct);

        Task<Testimonial?> GetByUserIdAsync(Guid userId, CancellationToken ct);

        Task AddAsync(Testimonial testimonial, CancellationToken ct);

        void Remove(Testimonial testimonial);

        IQueryable<Testimonial> Query();
    }
}
