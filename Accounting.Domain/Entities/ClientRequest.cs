using Accounting.Domain.Enums;
using Accounting.Domain.Exceptions;

namespace Accounting.Domain.Entities;

public sealed class ClientRequest
{
    public Guid Id { get; private set; }

    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? Message { get; private set; }
    public string? AdminNote { get; private set; }

    public Guid? ServiceId { get; private set; }
    public Guid? PricingPackageId { get; private set; }

    public RequestStatus Status { get; private set; }
    public RequestType RequestType { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    // EF Core needs it
    private ClientRequest() { }

    private ClientRequest(
        Guid id,
        string fullName,
        string email,
        string? phone,
        string? message,
        Guid? serviceId,
        Guid? pricingPackageId,
        RequestType requestType,
        RequestStatus status,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        Id = id;

        SetFullName(fullName);
        SetEmail(email);
        SetPhone(phone);
        SetMessage(message);

        SetSelection(requestType, serviceId, pricingPackageId);

        Status = status;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public static ClientRequest Create(
        string fullName,
        string email,
        string? phone,
        string? message,
        Guid? serviceId,
        Guid? pricingPackageId,
        RequestType requestType)
    {
        var now = DateTime.UtcNow;

        return new ClientRequest(
            Guid.NewGuid(),
            fullName,
            email,
            phone,
            message,
            serviceId,
            pricingPackageId,
            requestType,
            RequestStatus.New,
            now,
            now);
    }

    public void UpdateSelection(
        RequestType requestType,
        Guid? serviceId,
        Guid? pricingPackageId)
    {
        SetSelection(requestType, serviceId, pricingPackageId);
        Touch();
    }

    public void UpdateContactInfo(string fullName, string email, string? phone)
    {
        SetFullName(fullName);
        SetEmail(email);
        SetPhone(phone);
        Touch();
    }

    public void UpdateMessage(string? message)
    {
        SetMessage(message);
        Touch();
    }

    public void SetAdminNote(string? note)
    {
        note = note?.Trim();

        if (string.IsNullOrWhiteSpace(note))
        {
            AdminNote = null;
            Touch();
            return;
        }

        if (note.Length > 2000)
            throw new DomainException("Admin note max length is 2000.");

        AdminNote = note;
        Touch();
    }

    public void MarkInProgress()
    {
        if (Status != RequestStatus.New && Status != RequestStatus.WaitingForClient)
            throw new DomainException("Only new or waiting-for-client requests can be marked as in progress.");

        Status = RequestStatus.InProgress;
        Touch();
    }

    public void MarkWaitingForClient()
    {
        if (Status != RequestStatus.InProgress)
            throw new DomainException("Only in-progress requests can be marked as waiting for client.");

        Status = RequestStatus.WaitingForClient;
        Touch();
    }

    public void MarkCompleted()
    {
        if (Status != RequestStatus.InProgress && Status != RequestStatus.WaitingForClient)
            throw new DomainException("Only in-progress or waiting-for-client requests can be completed.");

        Status = RequestStatus.Completed;
        Touch();
    }

    public void MarkRejected()
    {
        if (Status != RequestStatus.New &&
            Status != RequestStatus.InProgress &&
            Status != RequestStatus.WaitingForClient)
            throw new DomainException("Only active requests can be rejected.");

        Status = RequestStatus.Rejected;
        Touch();
    }

    private void SetSelection(
        RequestType requestType,
        Guid? serviceId,
        Guid? pricingPackageId)
    {
        if (requestType == RequestType.Service)
        {
            if (serviceId is null || serviceId == Guid.Empty)
                throw new DomainException("Service id is required for service request.");

            if (pricingPackageId is not null)
                throw new DomainException("Pricing package id must be empty for service request.");
        }

        if (requestType == RequestType.Package)
        {
            if (pricingPackageId is null || pricingPackageId == Guid.Empty)
                throw new DomainException("Pricing package id is required for pricing package request.");

            if (serviceId is not null)
                throw new DomainException("Service id must be empty for pricing package request.");
        }

        if (requestType == RequestType.GeneralConsultation)
        {
            if (serviceId is not null || pricingPackageId is not null)
                throw new DomainException("General request cannot have service or pricing package selected.");
        }

        RequestType = requestType;
        ServiceId = serviceId;
        PricingPackageId = pricingPackageId;
    }

    private void SetFullName(string fullName)
    {
        fullName = fullName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name is required.");

        if (fullName.Length > 200)
            throw new DomainException("Full name max length is 200.");

        FullName = fullName;
    }

    private void SetEmail(string email)
    {
        Email = ValueObjects.Email.Create(email).Value;
    }

    private void SetPhone(string? phone)
    {
        phone = phone?.Trim();

        if (string.IsNullOrWhiteSpace(phone))
        {
            Phone = null;
            return;
        }

        if (phone.Length > 50)
            throw new DomainException("Phone max length is 50.");

        Phone = phone;
    }

    private void SetMessage(string? message)
    {
        message = message?.Trim();

        if (string.IsNullOrWhiteSpace(message))
        {
            Message = null;
            return;
        }

        if (message.Length > 4000)
            throw new DomainException("Message max length is 4000.");

        Message = message;
    }

    private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
}