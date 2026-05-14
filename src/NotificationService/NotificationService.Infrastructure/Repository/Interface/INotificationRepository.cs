using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Infrastructure.Repository.Interface
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification, CancellationToken ct);
        Task AddLogAsync(NotificationLog log, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
        Task<IEnumerable<Notification>> GetUnreadAsync(int userId, CancellationToken ct);
        Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct);
        Task UpdateAsync(Notification notification, CancellationToken ct);
        Task MarkAllAsReadAsync(int userId, CancellationToken ct);
    }
}
