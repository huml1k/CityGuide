using ContentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Domain.Interfaces.Repositories
{
    public interface ITagRepository
    {
        /// <summary>
        /// Получить тег по Id.
        /// </summary>
        Task<Tag?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить тег по имени.
        /// Нужно чтобы:
        /// - не создавать дубликаты
        /// - искать существующие теги
        /// </summary>
        Task<Tag?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить все теги.
        /// Используется для фильтров на frontend.
        /// </summary>
        Task<IReadOnlyCollection<Tag>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Создать новый тег.
        /// </summary>
        //Task AddAsync(
        //    Tag tag,
        //    CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверить существование тега.
        /// </summary>
        Task<bool> ExistsAsync(
            string name,
            CancellationToken cancellationToken = default);

        Task<List<Tag>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken);
    }
}
