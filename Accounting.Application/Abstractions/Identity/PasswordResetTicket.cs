namespace Accounting.Application.Abstractions.Identity
{
    /// <param name="Token">
    /// A one-time code from Identity. It contains characters that break inside
    /// a URL, so it goes into the link already encoded.
    /// </param>
    public sealed record PasswordResetTicket(string Email, string FullName, string Token);
}
