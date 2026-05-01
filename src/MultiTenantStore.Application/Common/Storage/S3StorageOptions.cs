namespace MultiTenantStore.Infrastructure.Storage;

public sealed class S3StorageOptions
{
    public string BucketName { get; set; } = default!;

    public string Region { get; set; } = default!;

    public string AccessKey { get; set; } = default!;

    public string SecretKey { get; set; } = default!;

    public string? PublicBaseUrl { get; set; }
}