using System.Collections.Generic;

namespace Accounting.Application.Features.ClientDocuments.Common
{
    public static class AllowedDocumentContentTypes
    {
        public const long MaxSizeBytes = 20 * 1024 * 1024;

        public static readonly IReadOnlySet<string> Values = new HashSet<string>(
            new[]
            {
                "application/pdf",
                "image/jpeg",
                "image/png",
                "image/webp",
                "image/heic",
                "text/plain",
                "text/csv",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "application/vnd.ms-excel",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "application/zip",
                "application/x-zip-compressed"
            },
            System.StringComparer.OrdinalIgnoreCase);
    }
}
