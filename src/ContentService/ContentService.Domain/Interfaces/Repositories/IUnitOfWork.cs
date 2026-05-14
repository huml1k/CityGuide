using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Unit Of Work отвечает
    /// за единое сохранение изменений.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Сохраняет ВСЕ изменения в БД
        /// одной транзакцией.
        /// </summary>
        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
