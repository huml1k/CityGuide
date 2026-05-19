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
        public List<Notification?> CreateFromContentEvent(ContentEventDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EventType)) return new();

            var routeName = string.IsNullOrWhiteSpace(dto.RouteTitle) ? $"Маршрут #{dto.RouteId}" : dto.RouteTitle;
            var notifications = new List<Notification>();
            var baseDate = dto.Timestamp == default ? DateTime.UtcNow : dto.Timestamp;

            (string? type, string? title, string? message) eventInfo = dto.EventType.ToLowerInvariant() switch
            {
                "approved" => (
                    "ContentApproved",
                    "Маршрут одобрен",
                    $"Ваш маршрут «{routeName}» успешно создан и теперь доступен пользователям."
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
                "rejected" => 
                ( 
                    "ContentRejected",
                    "Маршрут отклонен",
                    $"Ваш маршрут «{routeName}» не прошел модерацию."
                ),
                _ => (null, null, null)
            };

            if (eventInfo.type == null) return notifications;

            notifications.Add( new Notification
            {
                Id = ParseEventId(dto.EventId),
                UserId = dto.CreatorId, 
                Type = eventInfo.type,
                Title = eventInfo.title,
                Message = eventInfo.message,
                RelatedRouteId = dto.RouteId, 
                IsRead = false,
                CreatedAt = dto.Timestamp == default ? DateTime.UtcNow : dto.Timestamp
            });

            return notifications;
        }

        public List<Notification?> CreateFromFavoriteEvent(FavoriteEventDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EventType)) return new();

            var routeName = string.IsNullOrWhiteSpace(dto.RouteTitle) ? $"Маршрут #{dto.RouteId}" : dto.RouteTitle;
            var notifications = new List<Notification>();
            var baseDate = dto.Timestamp == default ? DateTime.UtcNow : dto.Timestamp;

            switch (dto.EventType.ToLowerInvariant()) 
            {
                case "favoriteadded":
                    notifications.Add(new Notification
                    { 
                        Id = Guid.NewGuid(),
                        UserId = dto.UserId,
                        Type = "FavoriteAdded",
                        Title = "Добавлено в избранное",
                        Message = $"Маршрут {routeName} успешно добавлено в ваше избранное",
                        RelatedRouteId = dto.RouteId,
                        IsRead = false,
                        CreatedAt= baseDate
                    });
                    notifications.Add(new Notification
                    {
                        Id = Guid.NewGuid(),
                        UserId = dto.CreatorId,
                        Type = "RouteFavorited",
                        Title = "Маршрут добавлен в избранное",
                        Message = $"Пользователь добавил ваш маршрут {routeName} в избранное",
                        RelatedRouteId = dto.RouteId,
                        IsRead = false,
                        CreatedAt = baseDate
                    });
                    break;

                case "favoriteremoved":
                    notifications.Add(new Notification
                    {
                        Id = Guid.NewGuid(),
                        UserId = dto.UserId,
                        Type = "FavoriteRemoved",
                        Title = "Удалено из избранного",
                        Message = $"Маршрут {routeName} удален из вашего избранного",
                        RelatedRouteId = dto.RouteId,
                        IsRead = false,
                        CreatedAt = baseDate
                    });
                    break;
            }

            return notifications;
        }

        public List<Notification> CreateFromModerationEvent(ModerationEventDto dto)
        {
            if (dto?.NewStatus is null) return new();

            var routeName = string.IsNullOrWhiteSpace(dto.RouteTitle) ? $"Маршрут #{dto.RouteId}" : dto.RouteTitle;
            var notifications = new List<Notification>();
            var baseDate = dto.Timestamp == default ? DateTime.UtcNow : dto.Timestamp;

            switch (dto.NewStatus.ToLowerInvariant().Trim())
            {
                case "approved":
                    notifications.Add(new Notification
                    {
                        Id = Guid.NewGuid(),
                        UserId = dto.CreatorId,
                        Type = "RouteApproved",
                        Title = "Маршрут одобрен модератором",
                        Message = $"Ваш маршрут «{routeName}» успешно прошел модерацию и доступен пользователям.",
                        RelatedRouteId = dto.RouteId,
                        IsRead = false,
                        CreatedAt = baseDate
                    });
                    break;
                case "rejected":
                    notifications.Add(new Notification
                    {
                        Id = Guid.NewGuid(),
                        UserId = dto.CreatorId,
                        Type = "RouteRejected",
                        Title = "Маршрут отклонен",
                        Message = $"Ваш маршрут «{routeName}» не прошел модерацию. Причина: {dto.Reason ?? "Нарушение правил публикаций"}.",
                        RelatedRouteId = dto.RouteId,
                        IsRead = false,
                        CreatedAt = baseDate
                    });
                    break;
            }

            return notifications;
        }

        private static Guid ParseEventId(string? eventId)
        {
            return !string.IsNullOrEmpty(eventId) && Guid.TryParse(eventId, out Guid guid)
                ? guid
                : Guid.NewGuid();
        }
    }
}
