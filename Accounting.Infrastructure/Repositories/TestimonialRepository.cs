using Accounting.Application.Abstractions.Persistence;
using Accounting.Domain.Entities;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Infrastructure.Repositories
{
    internal class TestimonialRepository : ITestimonialRepository
    {
        private readonly AppDbContext _dbContext;

        public TestimonialRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Testimonial?> GetByIdAsync(Guid id, CancellationToken ct)
            => await _dbContext.Testimonials.FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task<Testimonial?> GetByUserIdAsync(Guid userId, CancellationToken ct)
            => await _dbContext.Testimonials.FirstOrDefaultAsync(x => x.UserId == userId, ct);

        public async Task AddAsync(Testimonial testimonial, CancellationToken ct)
            => await _dbContext.Testimonials.AddAsync(testimonial, ct);

        public void Remove(Testimonial testimonial)
            => _dbContext.Testimonials.Remove(testimonial);

        public IQueryable<Testimonial> Query()
            => _dbContext.Testimonials.AsQueryable();
    }
}
