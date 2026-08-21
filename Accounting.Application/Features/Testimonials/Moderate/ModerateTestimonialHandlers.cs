using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common.Errors;
using Accounting.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Testimonials.Moderate
{
    /// <summary>
    /// Shared "load it or say it is missing". The three handlers below do this
    /// identically, and repeating the throw in each of them is pointless.
    /// </summary>
    internal static class TestimonialLoader
    {
        public static async Task<Testimonial> LoadAsync(
            ITestimonialRepository repository,
            Guid id,
            CancellationToken ct)
        {
            return await repository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException($"Testimonial {id} was not found.");
        }
    }

    public sealed class ApproveTestimonialHandler : IRequestHandler<ApproveTestimonialCommand>
    {
        private readonly ITestimonialRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ApproveTestimonialHandler(ITestimonialRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ApproveTestimonialCommand request, CancellationToken ct)
        {
            var testimonial = await TestimonialLoader.LoadAsync(_repository, request.Id, ct);

            testimonial.Approve();

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }

    public sealed class RejectTestimonialHandler : IRequestHandler<RejectTestimonialCommand>
    {
        private readonly ITestimonialRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RejectTestimonialHandler(ITestimonialRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RejectTestimonialCommand request, CancellationToken ct)
        {
            var testimonial = await TestimonialLoader.LoadAsync(_repository, request.Id, ct);

            testimonial.Reject(request.Note);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }

    public sealed class DeleteTestimonialHandler : IRequestHandler<DeleteTestimonialCommand>
    {
        private readonly ITestimonialRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTestimonialHandler(ITestimonialRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteTestimonialCommand request, CancellationToken ct)
        {
            var testimonial = await TestimonialLoader.LoadAsync(_repository, request.Id, ct);

            // A hard delete: afterwards the client can write a new testimonial,
            // because the unique index on UserId is freed.
            _repository.Remove(testimonial);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
