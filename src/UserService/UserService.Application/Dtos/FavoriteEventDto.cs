using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace UserService.Application.Dtos
{
    public class FavoriteEventDto
    {
        [JsonPropertyName("eventId")]
        public string EventId { get; init; } = Guid.NewGuid().ToString();

        [JsonPropertyName("eventType")]
        public string EventType { get; init; } = string.Empty;

        [JsonPropertyName("userId")]
        public Guid UserId { get; init; } 

        [JsonPropertyName("routeId")]
        public Guid RouteId { get; init; } 

        [JsonPropertyName("creatorId")]
        public Guid CreatorId { get; init; } 

        [JsonPropertyName("routeTitle")]
        public string? RouteTitle { get; init; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
