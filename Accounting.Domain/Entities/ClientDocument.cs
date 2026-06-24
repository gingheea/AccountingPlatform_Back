using Accounting.Domain.Enums;
using Accounting.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Domain.Entities
{
    public sealed class ClientDocument
    {
        private ClientDocument() { }

        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public string Title { get; private set; } = string.Empty;

        public string FileName { get; private set; } = string.Empty;

        public string FileUrl { get; private set; } = string.Empty;

        public string ContentType { get; private set; } = string.Empty;

        public long SizeBytes { get; private set; }

        public ClientDocumentCategory Category { get; private set; }

        public ClientDocumentDirection Direction { get; private set; }

        public ClientDocumentStatus Status { get; private set; }

        public string? Note { get; private set; }

        public DateTimeOffset CreatedAtUtc { get; private set; }

        public DateTimeOffset UpdatedAtUtc { get; private set; }

        public static ClientDocument Create(
            Guid userId,
            string title,
            string fileName,
            string fileUrl,
            string contentType,
            long sizeBytes,
            ClientDocumentCategory category,
            ClientDocumentDirection direction,
            string? note)
        {
            if (userId == Guid.Empty)
                throw new DomainException("User id is required.");

            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Document title is required.");

            if (string.IsNullOrWhiteSpace(fileName))
                throw new DomainException("File name is required.");

            if (string.IsNullOrWhiteSpace(fileUrl))
                throw new DomainException("File URL is required.");

            if (sizeBytes <= 0)
                throw new DomainException("File size must be greater than zero.");

            var now = DateTimeOffset.UtcNow;

            return new ClientDocument
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title.Trim(),
                FileName = fileName.Trim(),
                FileUrl = fileUrl.Trim(),
                ContentType = contentType.Trim(),
                SizeBytes = sizeBytes,
                Category = category,
                Direction = direction,
                Status = ClientDocumentStatus.Uploaded,
                Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };
        }

        public void MarkInReview()
        {
            Status = ClientDocumentStatus.InReview;
            Touch();
        }

        public void Approve()
        {
            Status = ClientDocumentStatus.Approved;
            Touch();
        }

        public void Reject(string? note)
        {
            Status = ClientDocumentStatus.Rejected;
            Note = string.IsNullOrWhiteSpace(note) ? Note : note.Trim();
            Touch();
        }

        public void Archive()
        {
            Status = ClientDocumentStatus.Archived;
            Touch();
        }

        public void UpdateNote(string? note)
        {
            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
            Touch();
        }

        private void Touch()
        {
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }
    }
}
