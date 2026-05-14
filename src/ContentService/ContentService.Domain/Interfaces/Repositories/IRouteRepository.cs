using ContentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Domain.Interfaces.Repositories
{
    public interface IRouteRepository
    {
        /// <summary>
        /// Получить маршрут по Id.
        /// Нужен для детальной страницы маршрута.
        /// </summary>
        Task<Route?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить маршрут вместе:
        /// - с точками
        /// - изображениями
        /// - аудио
        /// - тегами
        /// Используется для full route page.
        /// </summary>
        Task<Route?> GetFullRouteByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить список маршрутов.
        /// Используется для каталога/ленты.
        /// </summary>
        Task<IReadOnlyCollection<Route>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Поиск маршрутов по названию.
        /// </summary>
        Task<IReadOnlyCollection<Route>> SearchByTitleAsync(
            string search,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Фильтрация маршрутов по тегу.
        /// </summary>
        Task<IReadOnlyCollection<Route>> GetByTagAsync(
            Guid tagId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить маршруты конкретного автора.
        /// </summary>
        Task<IReadOnlyCollection<Route>> GetByCreatorIdAsync(
            Guid creatorId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Добавление нового маршрута.
        /// SaveChanges НЕ вызывается здесь.
        /// </summary>
        Task AddAsync(Route route, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновление маршрута.
        /// EF будет отслеживать изменения.
        /// </summary>
        void Update(Route route);

        /// <summary>
        /// Удаление маршрута.
        /// soft delete.
        /// </summary>
        void Delete(Route route);

        /// <summary>
        /// Проверка существования маршрута.
        /// </summary>
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
