using System.Net.Mime;
using ContentService.Application.Interfaces;
using ContentService.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace ContentService.Infrastructure.MinIo;

/// <summary>
/// Класс для взаимодействия с хранилищем MinIo
/// </summary>
public class MinIoFileStorage : IFileStorageService
{
    private readonly MinIoOptions _options;
    private readonly IMinioClient _client;
    private readonly int _timeLimitMinutes = 5; 

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
        await EnsureBucketExistsAsync(ct);
        
        var putArgs = new PutObjectArgs()
            .WithBucket(_options.ContentBucket)
            .WithObject(fileName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType);
        
        var x = await _client.PutObjectAsync(putArgs, ct);
        return x.ObjectName;
    }

    public async Task<Stream> DownloadFileAsync(string fileName, CancellationToken ct = default)
    {
        await EnsureBucketExistsAsync(ct);
        
        var args = new GetObjectArgs()
            .WithBucket(_options.ContentBucket)
            .WithFile(fileName);
        await _client.GetObjectAsync(args, ct);

        throw new NotImplementedException();
    }

    public async Task DeleteFileAsync(string fileName, CancellationToken ct = default)
    {
        await EnsureBucketExistsAsync(ct);
        
        await _client.RemoveObjectAsync(
            new RemoveObjectArgs()
                .WithBucket(_options.ContentBucket)
                .WithObject(fileName),
            ct);
    }

    public async Task<string> GetFileUrlAsync(string fileName, CancellationToken ct = default, TimeSpan? expiry = null)
    {
        await EnsureBucketExistsAsync(ct);
        
        var expirySeconds = (expiry ?? TimeSpan.FromMinutes(5)).TotalSeconds;

        var req = new PresignedGetObjectArgs()
            .WithBucket(_options.ContentBucket)
            .WithObject(fileName)
            .WithExpiry((int)expirySeconds);
        
        var url = await _client.PresignedGetObjectAsync(req);
        return url;
    }

    private async Task EnsureBucketExistsAsync(CancellationToken ct)
    {
        var beArgs = new BucketExistsArgs().WithBucket(_options.ContentBucket);
        if (!await _client.BucketExistsAsync(beArgs, ct))
        {
            var mbArgs = new MakeBucketArgs().WithBucket(_options.ContentBucket);
            await _client.MakeBucketAsync(mbArgs, ct);
        }
    }
}