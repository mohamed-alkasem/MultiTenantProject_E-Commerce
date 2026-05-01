using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using MultiTenantStore.Application.Common.Storage;

namespace MultiTenantStore.Infrastructure.Storage;

public sealed class S3FileStorageService : IFileStorageService
{
    private readonly S3StorageOptions _options;

    public S3FileStorageService(IOptions<S3StorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> UploadAsync(
        byte[] content,
        string fileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.BucketName))
        {
            throw new InvalidOperationException("S3 BucketName is not configured.");
        }

        if (string.IsNullOrWhiteSpace(_options.Region))
        {
            throw new InvalidOperationException("S3 Region is not configured.");
        }

        if (string.IsNullOrWhiteSpace(_options.AccessKey) ||
            string.IsNullOrWhiteSpace(_options.SecretKey))
        {
            throw new InvalidOperationException("S3 credentials are not configured.");
        }

        var credentials = new BasicAWSCredentials(
            _options.AccessKey,
            _options.SecretKey);

        var region = RegionEndpoint.GetBySystemName(_options.Region);

        using var client = new AmazonS3Client(credentials, region);

        var safeFolder = folder.Trim('/');

        var key = $"{safeFolder}/{Guid.NewGuid()}-{fileName}";

        await using var stream = new MemoryStream(content);

        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key,
            InputStream = stream,
            ContentType = contentType
        };

        await client.PutObjectAsync(request, cancellationToken);

        if (!string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
        {
            return $"{_options.PublicBaseUrl.TrimEnd('/')}/{key}";
        }

        return $"https://{_options.BucketName}.s3.{_options.Region}.amazonaws.com/{key}";
    }
}