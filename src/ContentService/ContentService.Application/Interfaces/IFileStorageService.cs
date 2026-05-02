namespace ContentService.Application.Interfaces;

public interface IFileStorageService
{
    /// <summary>
    /// Загрузить файл в хранилище
    /// </summary>
    /// <param name="stream">Stream файла</param>
    /// <param name="fileName">Имя файла</param>
    /// <param name="contentType">Расширение файла</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Имя файла</returns>
    Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);
    
    /// <summary>
    /// Скачать файл с хранилища
    /// </summary>
    /// <param name="fileName">Имя файла</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Stream файла</returns>
    Task<Stream> DownloadFileAsync(string fileName, CancellationToken ct = default);
    
    /// <summary>
    /// Удалить файл с хранилища
    /// </summary>
    /// <param name="fileName">Имя файла</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    Task DeleteFileAsync(string fileName, CancellationToken ct = default);
    
    /// <summary>
    /// Получить Url файла для установки
    /// </summary>
    /// <param name="fileName">Имя файла</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    Task<string> GetFileUrlAsync(string fileName, CancellationToken ct = default);
}