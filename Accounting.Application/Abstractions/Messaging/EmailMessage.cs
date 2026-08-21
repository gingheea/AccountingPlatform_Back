namespace Accounting.Application.Abstractions.Messaging
{
    /// <summary>
    /// A single email. A dedicated type rather than a pile of method parameters:
    /// fields accumulate over time (attachments, cc) without breaking callers.
    /// </summary>
    /// <param name="TextBody">
    /// Plain-text version. HTML-only mail is flagged as spam more often.
    /// </param>
    /// <param name="ReplyTo">
    /// Where a "Reply" click goes. For a new-request notification we put the
    /// client's address here so the accountant answers them directly.
    /// </param>
    public sealed record EmailMessage(
        string To,
        string Subject,
        string HtmlBody,
        string? TextBody = null,
        string? ReplyTo = null,
        string? ReplyToName = null
    );
}
