using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NotificationService.Application.DTOs
{
    public record UserEventDto
    {
        [JsonPropertyName("eventId")]
        public string? EventId { get; init; }

        [JsonPropertyName("eventType")]
        public string EventType { get; init; } = string.Empty;

        [JsonPropertyName("userId")]
        public Guid UserId { get; init; } 

        [JsonPropertyName("fullName")]
        public string? FullName { get; init; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; init; }
    }
}
