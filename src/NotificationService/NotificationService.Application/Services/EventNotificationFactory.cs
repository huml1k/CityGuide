using NotificationService.Application.DTOs;
using NotificationService.Application.Services.Interface;
using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Services
{
    public class EventNotificationFactory : IEventNotificationFactory
    {
        public Notification? CreateFromContentEvent(ContentEventDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EventType)) return null;

            var routeName = string.IsNullOrWhiteSpace(dto.RouteTitle) ? $"Маршрут #{dto.RouteId}" : dto.RouteTitle;

            var (type, title, message) = dto.EventType.ToLowerInvariant() switch
            {
                "approved" => (
                    "ContentApproved",
                    "Маршрут одобрен",
                    $"Ваш маршрут «{routeName}» успешно прошёл модерацию и теперь доступен пользователям."
                ),
                "updated" => (
                    "ContentUpdated",
                    "Маршрут изменён",
                    $"В маршрут «{routeName}» были внесены изменения. Проверьте актуальность данных."
                ),
                "deleted" => (
                    "ContentDeleted",
                    "Маршрут удалён",
                    $"Маршрут «{routeName}» был удалён модератором или системой."
                ),
                _ => (null, null, null)
            };

            if (type == null) return null;

            return new Notification
            {
                Id = ParseEventId(dto.EventId),
                UserId = dto.CreatorId, 
                Type = type,
                Title = title,
                Message = message,
                RelatedRouteId = (int)dto.RouteId, 
                IsRead = false,
                CreatedAt = dto.Timestamp == default ? DateTime.UtcNow : dto.Timestamp
            };
        }

        public Notification? CreateFromUserEvent(UserEventDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EventType)) return null;

            var name = string.IsNullOrWhiteSpace(dto.FullName) ? "пользователь" : dto.FullName;

            var (type, title, message) = dto.EventType.ToLowerInvariant() switch
            {
                "registered" => (
                    "UserRegistered",
                    "Добро пожаловать!",
                    $"Привет, {name}! Ваша регистрация успешно завершена."
                ),
                "profileupdated" => (
                    "UserProfileUpdated",
                    "Профиль обновлён",
                    "Изменения в вашем профиле успешно сохранены."
                ),
                "deleted" => (
                    "UserDeleted",
                    "Аккаунт удалён",
                    "Ваш аккаунт и все связанные данные были успешно удалены."
                ),
                _ => (null, null, null)
            };

            if (type == null) return null;

            return new Notification
            {
                Id = ParseEventId(dto.EventId),
                UserId = dto.UserId, 
                Type = type,
                Title = title,
                Message = message,
                RelatedRouteId = null,
                IsRead = false,
                CreatedAt = dto.Timestamp == default ? DateTime.UtcNow : dto.Timestamp
            };
        }

        private static Guid ParseEventId(string? eventId)
        {
            return !string.IsNullOrEmpty(eventId) && Guid.TryParse(eventId, out Guid guid)
                ? guid
                : Guid.NewGuid();
        }
    }
}
