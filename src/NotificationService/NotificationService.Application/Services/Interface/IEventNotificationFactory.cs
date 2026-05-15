using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Services.Interface
{
    public interface IEventNotificationFactory
    {
        public Notification? CreateFromUserEvent(UserEventDto dto);
        public Notification? CreateFromContentEvent(ContentEventDto dto);
    }
}
