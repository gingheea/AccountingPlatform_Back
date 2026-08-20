using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Abstractions.Persistence;
using Accounting.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Testimonials.Submit
{
    public sealed class SubmitTestimonialHandler : IRequestHandler<SubmitTestimonialCommand, Guid>
    {
        private readonly ITestimonialRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserManagementService _userManagementService;
        private readonly IUnitOfWork _unitOfWork;

        public SubmitTestimonialHandler(
            ITestimonialRepository repository,
            ICurrentUserService currentUserService,
            IUserManagementService userManagementService,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _userManagementService = userManagementService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(SubmitTestimonialCommand request, CancellationToken ct)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Current user is not authenticated.");

            var existing = await _repository.GetByUserIdAsync(userId, ct);

            // Один відгук на клієнта: повторне надсилання не створює другий запис,
            // а переписує наявний і повертає його на розгляд.
            if (existing is not null)
            {
                existing.UpdateContent(request.Content, request.AuthorRole);

                await _unitOfWork.SaveChangesAsync(ct);

                return existing.Id;
            }

            var user = await _userManagementService.GetPortalUserByIdAsync(userId, ct)
                ?? throw new UnauthorizedAccessException("Current user is not active or does not exist.");

            var testimonial = Testimonial.Create(
                userId,
                ResolveAuthorName(user.FullName, user.Email),
                request.AuthorRole,
                request.Content);

            await _repository.AddAsync(testimonial, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return testimonial.Id;
        }

        /// <summary>
        /// Ім'я в профілі не обов'язкове, а підпис під відгуком потрібен завжди.
        /// Якщо імені немає — беремо частину пошти до «@», щоб не впасти
        /// і не підписати відгук порожнім рядком.
        /// </summary>
        private static string ResolveAuthorName(string? fullName, string email)
        {
            if (!string.IsNullOrWhiteSpace(fullName))
                return fullName;

            var at = email.IndexOf('@');

            return at > 0 ? email[..at] : email;
        }
    }
}
