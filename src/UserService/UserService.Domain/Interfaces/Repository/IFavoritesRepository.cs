using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces;

public interface IFavoritesRepository
{
    /// <summary>
    /// Получить избранные пути для пользователя
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<Guid>> GetUserFavoritesAsync(Guid userId, CancellationToken ct = default); 
    
    /// <summary>
    /// Получить все избранные для определенного пути
    /// </summary>
    /// <param name="routeId">Id пути</param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<Guid>> GetRouteFavoritesAsync(Guid routeId, CancellationToken ct = default); 
    
    /// <summary>
    /// Добавить избранные
    /// </summary>
    /// <param name="favorite">Избранные</param>
    /// <returns></returns>
    public Task AddFavoriteAsync(Favorite favorite, CancellationToken ct);
    
    /// <summary>
    /// Удалить избранное
    /// </summary>
    /// <param name="favorite"></param>
    /// <returns></returns>
    public Task DeleteFavoriteAsync(Favorite favorite, CancellationToken ct = default);
    
    /// <summary>
    /// Проверить есть ли путь в избранных у пользователя
    /// </summary>
    /// <param name="routeId">Путь</param>
    /// <param name="userId">Пользователь</param>
    /// <returns></returns>
    public Task<bool> IsFavoriteAsync(Guid routeId, Guid userId, CancellationToken ct);
    
    /// <summary>
    /// Получить все избранные в базе данных
    /// </summary>
    /// <returns></returns>
    public Task<IReadOnlyCollection<Favorite>> GetAllFavoritesAsync(); 
    
    /// <summary>
    /// Получить определенный экземпляр по ID пути и профиля
    /// </summary>
    /// <param name="routeId">ID пути</param>
    /// <param name="userId">ID профиля</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Экземпляр Favorite</returns>
    Task<Favorite?> GetByUserAndRouteAsync(Guid routeId, Guid userId, CancellationToken cancellationToken = default);
}