using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NotificationService.Application.DTOs
{
    public record ModerationEventDto
    {
        [JsonPropertyName("eventId")]
        public string EventId { get; init; } = Guid.NewGuid().ToString();

        [JsonPropertyName("routeId")]
        public Guid RouteId { get; init; }

        [JsonPropertyName("creatorId")]
        public Guid CreatorId { get; init; } 

        [JsonPropertyName("routeTitle")]
        public string? RouteTitle { get; init; }

        [JsonPropertyName("newStatus")]
        public string NewStatus { get; init; } = string.Empty; // "approved" или "rejected"

        [JsonPropertyName("reason")]
        public string? Reason { get; init; } 

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
