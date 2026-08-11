using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.Storage
{
    public interface IFileStorage
    {
        Task UploadAsync(
            string storageKey,
            Stream content,
            string contentType,
            CancellationToken ct);

        /// <summary>
        /// Returns a short-lived pre-signed URL. The file itself never goes through the API.
        /// </summary>
        Task<string> GetDownloadUrlAsync(
            string storageKey,
            string downloadFileName,
            TimeSpan lifetime,
            CancellationToken ct);

        Task DeleteAsync(string storageKey, CancellationToken ct);
    }
}
