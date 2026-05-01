namespace MultiTenantStore.Application.Common.Storage;

public interface IFileStorageService
{
    Task<string> UploadAsync(
        byte[] content,
        string fileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken = default);
}