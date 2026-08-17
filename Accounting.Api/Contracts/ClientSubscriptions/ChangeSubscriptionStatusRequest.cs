using Accounting.Domain.Enums;

namespace Accounting.Api.Contracts.ClientSubscriptions
{
    public sealed record ChangeSubscriptionStatusRequest(SubscriptionStatus Status);
}
