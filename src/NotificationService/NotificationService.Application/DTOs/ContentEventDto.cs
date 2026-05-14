using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NotificationService.Application.DTOs
{
    public record ContentEventDto
    {
        [JsonPropertyName("eventId")]
        public string? EventId { get; init; }

        [JsonPropertyName("eventType")]
        public string EventType { get; init; } = string.Empty;

        [JsonPropertyName("routeId")]
        public long RouteId { get; init; }

        [JsonPropertyName("creatorId")]
        public Guid CreatorId { get; init; } 

        [JsonPropertyName("routeTitle")]
        public string? RouteTitle { get; init; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; init; }
    }
}
