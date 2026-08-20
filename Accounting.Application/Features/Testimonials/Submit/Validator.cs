using Accounting.Domain.Entities;
using FluentValidation;

namespace Accounting.Application.Features.Testimonials.Submit
{
    public class Validator : AbstractValidator<SubmitTestimonialCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Напишіть, будь ласка, кілька слів.")
                .MinimumLength(Testimonial.MinContentLength)
                    .WithMessage($"Відгук має містити щонайменше {Testimonial.MinContentLength} символів.")
                .MaximumLength(Testimonial.MaxContentLength)
                    .WithMessage($"Максимальна довжина відгуку — {Testimonial.MaxContentLength} символів.");

            RuleFor(x => x.AuthorRole)
                .MaximumLength(100).WithMessage("Максимальна довжина — 100 символів.");
        }
    }
}
