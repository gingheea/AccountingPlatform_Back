namespace Accounting.Infrastructure.Storage
{
    public sealed class S3StorageOptions
    {
        /// <summary>
        /// For Cloudflare R2: https://&lt;account-id&gt;.r2.cloudflarestorage.com
        /// </summary>
        public string ServiceUrl { get; set; } = string.Empty;

        /// <summary>"auto" for R2, e.g. "eu-central-1" for real AWS S3.</summary>
        public string Region { get; set; } = "auto";

        public string BucketName { get; set; } = string.Empty;

        public string AccessKeyId { get; set; } = string.Empty;

        public string SecretAccessKey { get; set; } = string.Empty;

        /// <summary>R2 and MinIO require path-style addressing.</summary>
        public bool ForcePathStyle { get; set; } = true;
    }
}
