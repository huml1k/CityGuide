using ContentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Domain.Interfaces.Repositories
{
    public interface IRouteReviewRepository
    {
        /// <summary>
        /// Получить отзыв по Id.
        /// </summary>
        Task<RouteReview?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить все отзывы маршрута.
        /// Используется на странице маршрута.
        /// </summary>
        Task<IReadOnlyCollection<RouteReview>> GetByRouteIdAsync(
            Guid routeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить отзыв пользователя
        /// для конкретного маршрута.
        ///
        /// Нужен потому что:
        /// один пользователь может оставить
        /// только один отзыв.
        /// </summary>
        Task<RouteReview?> GetUserReviewAsync(
            Guid routeId,
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить средний рейтинг маршрута.
        /// Используется для статистики.
        /// </summary>
        Task<double> GetAverageRatingAsync(
            Guid routeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить количество отзывов.
        /// </summary>
        Task<int> GetReviewsCountAsync(
            Guid routeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверить,
        /// оставлял ли пользователь отзыв.
        /// </summary>
        Task<bool> ExistsAsync(
            Guid routeId,
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Создать новый отзыв.
        /// </summary>
        Task AddAsync(
            RouteReview review,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновить отзыв.
        /// </summary>
        void Update(RouteReview review);

        /// <summary>
        /// Удалить отзыв.
        /// </summary>
        void Delete(RouteReview review);
    }
}
