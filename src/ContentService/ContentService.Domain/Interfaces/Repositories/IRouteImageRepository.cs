using ContentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Domain.Interfaces.Repositories
{
    public interface IRouteImageRepository
    {
        /// <summary>
        /// Загрузка метаданных
        /// </summary>
        Task AddAsync( RouteImage image, CancellationToken cancellationToken = default);

        /// <summary>
        /// Для скачивания/удаления файла
        /// </summary>
        Task<RouteImage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получение всех файлов маршрута
        /// </summary>
        Task<IReadOnlyCollection<RouteImage>> GetByRouteIdAsync(Guid routeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удаление метаданных
        /// </summary>
        void Delete(RouteImage image);
    }
}
