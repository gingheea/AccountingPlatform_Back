using Accounting.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Accounting.Api.Contracts.ClientDocuments
{
    public sealed class UploadMyDocumentRequest
    {
        public IFormFile? File { get; set; }

        public string Title { get; set; } = string.Empty;

        public ClientDocumentCategory Category { get; set; }

        public string? Note { get; set; }
    }
}
