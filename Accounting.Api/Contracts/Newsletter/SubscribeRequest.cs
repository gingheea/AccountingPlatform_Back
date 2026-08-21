namespace Accounting.Api.Contracts.Newsletter
{
    /// <param name="Source">Where the signup came from: footer, home, blog.</param>
    public sealed record SubscribeRequest(string Email, string? Source);
}
