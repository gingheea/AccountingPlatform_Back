namespace Accounting.Api.Contracts.Testimonials
{
    /// <param name="AuthorRole">The author's occupation: "sole trader", "shop owner".</param>
    public sealed record SubmitTestimonialRequest(string Content, string? AuthorRole);

    /// <param name="Note">Rejection reason; the author sees it in their portal.</param>
    public sealed record RejectTestimonialRequest(string? Note);
}
