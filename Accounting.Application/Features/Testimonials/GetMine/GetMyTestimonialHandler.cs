using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Features.Testimonials.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Testimonials.GetMine
{
    public sealed class GetMyTestimonialHandler : IRequestHandler<GetMyTestimonialQuery, TestimonialDto?>
    {
        private readonly ITestimonialRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetMyTestimonialHandler(
            ITestimonialRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<TestimonialDto?> Handle(GetMyTestimonialQuery request, CancellationToken ct)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Current user is not authenticated.");

            var testimonial = await _repository.GetByUserIdAsync(userId, ct);

            if (testimonial is null)
                return null;

            return new TestimonialDto(
                testimonial.Id,
                testimonial.UserId,
                testimonial.AuthorName,
                testimonial.AuthorRole,
                testimonial.Content,
                testimonial.Status,
                testimonial.ModerationNote,
                testimonial.CreatedAtUtc,
                testimonial.ModeratedAtUtc);
        }
    }
}
