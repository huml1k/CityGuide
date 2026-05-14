using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Infrastructure.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationRepository(NotificationDbContext context) 
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification, CancellationToken ct)
        {
            await _context.Notifications.AddAsync(notification, ct);
        }

        public async Task AddLogAsync(NotificationLog log, CancellationToken ct)
        {
            await _context.NotificationLogs.AddAsync(log, ct);
        }

        public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Notifications.FindAsync([id], cancellationToken: ct);
        }

        public async Task<IEnumerable<Notification>> GetUnreadAsync(int userId, CancellationToken ct)
        {
            return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .ToListAsync(cancellationToken: ct);
        }

        public async Task MarkAllAsReadAsync(int userId, CancellationToken ct)
        {
            await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(n => n.IsRead, true), ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Notification notification, CancellationToken ct)
        {
            _context.Notifications.Update(notification);
            await Task.CompletedTask;
        }
    }
}
