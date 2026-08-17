using Accounting.Domain.Enums;
using Accounting.Domain.Exceptions;

namespace Accounting.Domain.Entities;

/// <summary>
/// Обслуговування клієнта: який пакет або послугу бухгалтер веде для нього
/// зараз. Відрізняється від заявки — заявка це разова подія «людина запитала»,
/// а це тривалі стосунки, які починаються, можуть паузитись і завершуються.
/// </summary>
public sealed class ClientSubscription
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public Guid? ServiceId { get; private set; }
    public Guid? PricingPackageId { get; private set; }

    public SubscriptionStatus Status { get; private set; }

    public DateTime StartedAtUtc { get; private set; }
    public DateTime? EndedAtUtc { get; private set; }

    public string? Note { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    // EF Core потребує безпараметричний конструктор
    private ClientSubscription() { }

    public static ClientSubscription Create(
        Guid userId,
        Guid? serviceId,
        Guid? pricingPackageId,
        DateTime startedAtUtc,
        string? note)
    {
        if (userId == Guid.Empty)
            throw new DomainException("User id is required.");

        SetSelectionGuard(serviceId, pricingPackageId);

        var now = DateTime.UtcNow;

        return new ClientSubscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ServiceId = serviceId,
            PricingPackageId = pricingPackageId,
            Status = SubscriptionStatus.Active,
            StartedAtUtc = startedAtUtc == default ? now : startedAtUtc,
            Note = Normalize(note),
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };
    }

    public void Pause()
    {
        if (Status != SubscriptionStatus.Active)
            throw new DomainException("Only an active subscription can be paused.");

        Status = SubscriptionStatus.Paused;
        Touch();
    }

    public void Resume()
    {
        if (Status != SubscriptionStatus.Paused)
            throw new DomainException("Only a paused subscription can be resumed.");

        Status = SubscriptionStatus.Active;
        Touch();
    }

    public void End(DateTime? endedAtUtc = null)
    {
        if (Status == SubscriptionStatus.Ended)
            throw new DomainException("Subscription is already ended.");

        var moment = endedAtUtc ?? DateTime.UtcNow;

        if (moment < StartedAtUtc)
            throw new DomainException("End date cannot be earlier than the start date.");

        Status = SubscriptionStatus.Ended;
        EndedAtUtc = moment;
        Touch();
    }

    public void UpdateNote(string? note)
    {
        Note = Normalize(note);
        Touch();
    }

    /// <summary>
    /// Або послуга, або пакет — рівно одне. Обидва означали б незрозуміло
    /// що коштує, жодного — обслуговування без предмета.
    /// </summary>
    private static void SetSelectionGuard(Guid? serviceId, Guid? pricingPackageId)
    {
        var hasService = serviceId is not null && serviceId != Guid.Empty;
        var hasPackage = pricingPackageId is not null && pricingPackageId != Guid.Empty;

        if (hasService == hasPackage)
            throw new DomainException("Exactly one of service or pricing package must be selected.");
    }

    private static string? Normalize(string? note)
    {
        note = note?.Trim();

        if (string.IsNullOrWhiteSpace(note))
            return null;

        if (note.Length > 2000)
            throw new DomainException("Note max length is 2000.");

        return note;
    }

    private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
}
