namespace Accounting.Api.Contracts.Testimonials
{
    /// <param name="AuthorRole">Рід занять автора: «ФОП», «власниця магазину».</param>
    public sealed record SubmitTestimonialRequest(string Content, string? AuthorRole);

    /// <param name="Note">Причина відхилення — її побачить автор у своєму кабінеті.</param>
    public sealed record RejectTestimonialRequest(string? Note);
}
