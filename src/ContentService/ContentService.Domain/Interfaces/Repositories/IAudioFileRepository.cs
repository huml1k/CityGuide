using ContentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Domain.Interfaces.Repositories
{
    public interface IAudioFileRepository
    {
        /// <summary>
        /// Загрузка метаданных
        /// </summary>
        Task AddAsync(AudioFile audioFile, CancellationToken cancellationToken = default);

        /// <summary>
        /// Для скачивания/удаления файла
        /// </summary>
        Task<AudioFile?> GetByIdAsync( Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получение всех файлов маршрута
        /// </summary>
        Task<IReadOnlyCollection<AudioFile>> GetByRouteIdAsync(Guid routeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удаление метаданных
        /// </summary>
        void Delete(AudioFile audioFile);
    }
}
