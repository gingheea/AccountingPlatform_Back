using System;

namespace Accounting.Application.Features.ClientDocuments.Common
{
    public sealed record DocumentDownloadDto(
        string Url,
        string FileName,
        DateTimeOffset ExpiresAtUtc
    );
}
