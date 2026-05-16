using ContentService.Application.Interfaces;

namespace ContentService.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IFileStorageService _storage;
    private readonly IFileRepository _repository;

    public Task DeleteFileAsync(string fileName, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> DownloadFileAsync(string fileName, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetFileUrlAsync(string fileName, CancellationToken ct = default, TimeSpan? expiry = null)
    {
        throw new NotImplementedException();
    }

    public Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}