using Accounting.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;

namespace Accounting.Api.Contracts.ClientDocuments
{
    public sealed class UploadDocumentRequest
    {
        public IFormFile? File { get; set; }

        public Guid UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public ClientDocumentCategory Category { get; set; }

        public ClientDocumentDirection Direction { get; set; }

        public string? Note { get; set; }
    }
}
