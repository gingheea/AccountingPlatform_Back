using Accounting.Domain.Enums;
using Accounting.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Accounting.Domain.Entities
{
    public sealed class ClientRequest
    {
        public Guid Id { get; private set; }
        public string FullName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string? Phone { get; private set; }
        public string? Message { get; private set; }
        public string? AdminNote { get; private set; }
        public Guid? ServiceId { get; private set; }

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
       string message,
       Guid? serviceId,
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

            ServiceId = serviceId;
            RequestType = requestType;
            Status = status;

            CreatedAtUtc = createdAtUtc;
            UpdatedAtUtc = updatedAtUtc;
        }

        public static ClientRequest Create(
       string fullName, string email,
       string? phone, string message,
       Guid? serviceId, RequestType requestType)
        {
            var now = DateTime.UtcNow;

            return new ClientRequest(
                Guid.NewGuid(),
                fullName,
                email,
                phone,
                message,
                serviceId,
                requestType,
                RequestStatus.New,
                now,
                now
            );
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
            message = message?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(message))
                throw new DomainException("Message is required.");

            if (message.Length > 4000)
                throw new DomainException("Phone max length is 4000.");

            Message = message;
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


        public void UpdateContactInfo(string fullName, string email, string? phone)
        {
            SetFullName(fullName);
            SetEmail(email);
            SetPhone(phone);
            Touch();
        }


        private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
    }
}
