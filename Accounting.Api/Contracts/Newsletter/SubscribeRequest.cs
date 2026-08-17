namespace Accounting.Api.Contracts.Newsletter
{
    /// <param name="Source">Звідки підписались: footer, home, blog.</param>
    public sealed record SubscribeRequest(string Email, string? Source);
}
