using Mapster;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Config
{
    public static class MappingConfig
    {
        public static void RegisterMappings() 
        {
            TypeAdapterConfig<KafkaPushNotificationDto, Notification>
            .NewConfig()
            .Map(dest => dest.Id, src => ParseEventId(src.EventId))
            .Map(dest => dest.Type, _ => "Push") 
            .Map(dest => dest.Message, src => src.Body) 
            .Map(dest => dest.IsRead, _ => false)
            .Map(dest => dest.CreatedAt, src =>
                src.CreatedAt == default ? DateTime.UtcNow : src.CreatedAt)
            .IgnoreNonMapped(true); 

            TypeAdapterConfig<Notification, KafkaPushNotificationDto>
                .NewConfig()
                .Map(dest => dest.EventId, src => src.Id.ToString())
                .Map(dest => dest.Body, src => src.Message)
                .IgnoreNonMapped(true);
        }

        private static Guid ParseEventId(string eventId)
        {
            return !string.IsNullOrEmpty(eventId) && Guid.TryParse(eventId, out Guid guid)
                ? guid
                : Guid.NewGuid();
        }
    }
}
