using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Services.Interface
{
    public interface IEventNotificationFactory
    {
        public List<Notification?> CreateFromFavoriteEvent(FavoriteEventDto dto);
        public List<Notification?> CreateFromContentEvent(ContentEventDto dto);
        public List<Notification> CreateFromModerationEvent(ModerationEventDto dto);
    }
}
