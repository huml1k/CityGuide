using Confluent.Kafka;
using ContentService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ContentService.Infrastructure.Services
{
    public class KafkaEventPublisher : IKafkaEventPublisher
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaEventPublisher> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public KafkaEventPublisher(
            IProducer<string, string> producer,
            ILogger<KafkaEventPublisher> logger) 
        {
            _producer = producer;
            _logger = logger;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task PublishAsync<T>(string topic, T @event, CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(@event, _jsonOptions);

            var message = new Message<string, string> 
            {
                Key = Guid.NewGuid().ToString(),
                Value = json
            };

            try
            {
                var result = await _producer.ProduceAsync(topic, message, ct);
                _logger.LogDebug("Event published to {Topic} at offset {Offset}", topic, result.Offset);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(ex, "Failed to publish event to {Topic}. Error: {Error}", topic, ex.Error.Reason);
            }
        }
    }
}
