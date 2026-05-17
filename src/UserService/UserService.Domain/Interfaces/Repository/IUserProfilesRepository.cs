using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces;

public interface IUserProfilesRepository
{
    /// <summary>
    /// Получить всех пользователей в базе
    /// </summary>
    /// <returns></returns>
    public Task<IReadOnlyCollection<UserProfile>> GetAllAsync();
    
    /// <summary>
    /// Получить пользователя по его id
    /// </summary>
    /// <param name="id">ID пользователя</param>
    /// <returns></returns>
    public Task<UserProfile?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Создать пользователя
    /// </summary>
    /// <param name="profile">Данные для профиля</param>
    /// <returns></returns>
    public Task<Guid> CreateAsync(UserProfile profile);
    
    /// <summary>
    /// Обновить данные профиля
    /// </summary>
    /// <param name="profile">Новые данные
    /// </param>
    /// <returns></returns>
    public Task UpdateAsync(UserProfile profile);
    
    /// <summary>
    /// Удалить профиль
    /// </summary>
    /// <param name="id">ID пользователя</param>
    /// <returns></returns>
    public Task DeleteAsync(Guid id);
}