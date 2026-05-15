using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces;

public interface IFavoritesRepository
{
    /// <summary>
    /// Получить избранные пути для пользователя
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<Favorite>> GetUserFavoritesAsync(Guid userId); 
    
    /// <summary>
    /// Получить все избранные для определенного пути
    /// </summary>
    /// <param name="routeId">Id пути</param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<Favorite>> GetRouteFavoritesAsync(Guid routeId); 
    
    /// <summary>
    /// Добавить избранные
    /// </summary>
    /// <param name="favorite">Избранные</param>
    /// <returns></returns>
    public Task AddFavoriteAsync(Favorite favorite);
    
    /// <summary>
    /// Удалить избранное
    /// </summary>
    /// <param name="favorite"></param>
    /// <returns></returns>
    public void Delete(Favorite favorite);
    
    /// <summary>
    /// Проверить есть ли путь в избранных у пользователя
    /// </summary>
    /// <param name="routeId">Путь</param>
    /// <param name="userId">Пользователь</param>
    /// <returns></returns>
    public Task<bool> IsFavoriteAsync(Guid routeId, Guid userId);
    
    /// <summary>
    /// Получить все избранные в базе данных
    /// </summary>
    /// <returns></returns>
    public Task<IReadOnlyCollection<Favorite>> GetAllFavoritesAsync(); 
}