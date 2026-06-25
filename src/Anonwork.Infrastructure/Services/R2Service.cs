    using System.Net;
    using Anonwork.Application.Interfaces;
    using Anonwork.Infrastructure.Common;
    using Amazon.Runtime;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    namespace Anonwork.Infrastructure.Services;

    public class R2Service : IR2Service
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".svg", ".ico", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".mp4", ".mp3"
        };

        private const long MaxFileSize = 50 * 1024 * 1024;

        private readonly R2Options _options;
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger<R2Service> _logger;

        public R2Service(IOptions<R2Options> options, ILogger<R2Service> logger)
        {
            _options = options.Value;
            _options.Validate();
            _logger = logger;

            var credentials = new BasicAWSCredentials(_options.AccessKeyId, _options.SecretAccessKey);
            var config = new AmazonS3Config
            {
                ServiceURL = _options.Endpoint,
                ForcePathStyle = true,
                AuthenticationRegion = _options.Region,
                UseHttp = _options.Endpoint.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            };
            config.ResignRetries = false;

            _s3Client = new AmazonS3Client(credentials, config);
        }

        public async Task<(string FileKey, string FileUrl)> UploadFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
        {
            ValidateFile(file);
            await using var stream = file.OpenReadStream();
            return await UploadFileAsync(stream, file.FileName, file.ContentType, folder, cancellationToken);
        }

        public async Task<(string FileKey, string FileUrl)> UploadFileAsync(Stream stream, string fileName, string contentType, string folder, CancellationToken cancellationToken = default)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("File name is required.", nameof(fileName));

            var fileKey = BuildFileKey(folder, fileName);

            var putRequest = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = fileKey,
                InputStream = stream,
                ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType,
                DisablePayloadSigning = true,
                AutoCloseStream = false
            };

            try
            {
                await _s3Client.PutObjectAsync(putRequest, cancellationToken);
                var url = GetPublicUrl(fileKey);
                return (fileKey, url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to R2: {FileName}", fileName);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileKey)) return false;

            try
            {
                var response = await _s3Client.DeleteObjectAsync(_options.BucketName, fileKey, cancellationToken);
                return response.HttpStatusCode is HttpStatusCode.NoContent or HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file from R2: {FileKey}", fileKey);
                throw;
            }
        }

        public string GetPublicUrl(string fileKey)
        {
            if (string.IsNullOrWhiteSpace(fileKey))
                throw new ArgumentException("File key is required.", nameof(fileKey));

            if (!string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
                return $"{_options.PublicBaseUrl.TrimEnd('/')}/{fileKey.TrimStart('/')}";

            return $"{_options.Endpoint.TrimEnd('/')}/{_options.BucketName}/{fileKey.TrimStart('/')}";
        }

        private string BuildFileKey(string folder, string fileName)
        {
            var safeFolder = NormalizeFolder(folder);
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var guid = Guid.NewGuid().ToString("N");

            return string.IsNullOrWhiteSpace(safeFolder)
                ? $"{guid}{extension}"
                : $"{safeFolder}/{guid}{extension}";
        }

        private static string NormalizeFolder(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
                return string.Empty;

            return folder.Trim()
                .Trim('/')
                .Replace('\\', '/');
        }

        private static void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty", nameof(file));

            if (file.Length > MaxFileSize)
                throw new ArgumentException("File size exceeds maximum allowed size.", nameof(file));

            var extension = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException($"File type '{extension}' is not allowed.", nameof(file));
        }

    public string GetDefaultAvatarKey()
    {
        return _options.DefaultAvatarKey;
    }
}
