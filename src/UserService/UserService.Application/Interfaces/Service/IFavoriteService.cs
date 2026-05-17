namespace UserService.Application.Interfaces.Service;

public interface IFavoriteService
{
    /// <summary>
    /// Добавить маршрут в избранное текущего пользователя
    /// </summary>
    /// <returns>Id созданной записи избранного</returns>
    Task<Guid> AddFavoriteAsync(Guid routeId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удалить маршрут из избранного текущего пользователя
    /// </summary>
    Task RemoveFavoriteAsync(Guid routeId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверить, добавлен ли маршрут в избранное текущего пользователя
    /// </summary>
    Task<bool> IsFavoriteAsync(Guid routeId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить все избранные маршруты текущего пользователя
    /// </summary>
    Task<IReadOnlyCollection<Guid>> GetUserFavoriteRouteIdsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверить несколько маршрутов сразу
    /// </summary>
    /// <returns>Словарь: routeId -> isFavorite</returns>
    Task<Dictionary<Guid, bool>> CheckMultipleFavoritesAsync(
        IEnumerable<Guid> routeIds, 
        CancellationToken cancellationToken = default);
}