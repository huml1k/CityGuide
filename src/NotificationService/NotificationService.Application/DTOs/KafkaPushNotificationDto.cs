using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NotificationService.Application.DTOs
{
    public record KafkaPushNotificationDto
    {
        [JsonPropertyName("eventId")]
        public string? EventId { get; init; }

        [JsonPropertyName("userId")]
        public int UserId { get; init; }

        [JsonPropertyName("title")]
        public string Title { get; init; } = string.Empty;

        [JsonPropertyName("body")]
        public string Body { get; init; } = string.Empty;

        [JsonPropertyName("relatedRouteId")]
        public int? RelatedRouteId { get; init; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; init; }
    }
}
