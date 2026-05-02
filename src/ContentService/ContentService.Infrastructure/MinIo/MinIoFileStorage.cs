using System.Net.Mime;
using ContentService.Application.Interfaces;
using ContentService.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace ContentService.Infrastructure.MinIo;

public class MinIoFileStorage : IFileStorageService
{
    private readonly MinIoOptions _options;
    private readonly IMinioClient _client;

    public MinIoFileStorage(IOptions<MinIoOptions> options)
    {
        _options = options.Value;
        _client = new MinioClient()
            .WithEndpoint(_options.Endpoint)
            .WithCredentials(_options.AccessKey, _options.SecretKey)
            .Build();
    }
    
    public async Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        var beArgs = new BucketExistsArgs().WithBucket(_options.ContentBucket);
        if (!await _client.BucketExistsAsync(beArgs, ct))
        {
            var mbArgs = new MakeBucketArgs().WithBucket(_options.ContentBucket);
            await _client.MakeBucketAsync(mbArgs, ct);
        }
        
        var putArgs = new PutObjectArgs()
            .WithBucket(_options.ContentBucket)
            .WithObject(fileName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType);
        
        var x = await _client.PutObjectAsync(putArgs, ct);
        return x.ObjectName;
    }

    public Task<Stream> DownloadFileAsync(string fileName, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteFileAsync(string fileName, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetFileUrlAsync(string fileName, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}