namespace ContentService.Infrastructure.MinIo;

public class MinIoOptions
{
    public string Endpoint { get; set; } = "localhost:9000";
    public string AccessKey { get; set; } = "minioadmin";
    public string SecretKey { get; set; } = "minioadmin";
    public string ContentBucket { get; set; } = "content-files";
    public bool UseSsl { get; set; } = false;
}