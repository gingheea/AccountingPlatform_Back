using Accounting.Application.Abstractions.Storage;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Infrastructure.Storage
{
    internal sealed class S3FileStorage : IFileStorage
    {
        private readonly IAmazonS3 _s3;
        private readonly S3StorageOptions _options;

        public S3FileStorage(IAmazonS3 s3, IOptions<S3StorageOptions> options)
        {
            _s3 = s3;
            _options = options.Value;
        }

        public async Task UploadAsync(
            string storageKey,
            Stream content,
            string contentType,
            CancellationToken ct)
        {
            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = storageKey,
                InputStream = content,
                ContentType = contentType,

                // R2 has no STREAMING-AWS4-HMAC-SHA256-PAYLOAD, which the SDK uses by
                // default: send the body in one piece and sign the headers only. The
                // payload still travels over TLS, and SigV4 still authenticates the request.
                UseChunkEncoding = false,
                DisablePayloadSigning = true
            };

            await _s3.PutObjectAsync(request, ct);
        }

        public Task<string> GetDownloadUrlAsync(
            string storageKey,
            string downloadFileName,
            TimeSpan lifetime,
            CancellationToken ct)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = storageKey,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.Add(lifetime),
                ResponseHeaderOverrides = new ResponseHeaderOverrides
                {
                    ContentDisposition = BuildContentDisposition(downloadFileName)
                }
            };

            return _s3.GetPreSignedURLAsync(request);
        }

        public async Task DeleteAsync(string storageKey, CancellationToken ct)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = storageKey
            };

            await _s3.DeleteObjectAsync(request, ct);
        }

        /// <summary>
        /// Cyrillic file names are common here, so the name is sent twice: a plain ASCII
        /// fallback plus the RFC 5987 encoded form that modern browsers prefer.
        /// </summary>
        private static string BuildContentDisposition(string fileName)
        {
            var ascii = new StringBuilder(fileName.Length);

            foreach (var c in fileName)
                ascii.Append(c is >= (char)32 and < (char)127 && c != '"' && c != '\\' ? c : '_');

            var encoded = Uri.EscapeDataString(fileName);

            return $"attachment; filename=\"{ascii}\"; filename*=UTF-8''{encoded}";
        }
    }
}
