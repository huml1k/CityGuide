using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationService.Infrastructure.Services.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace NotificationService.Infrastructure.Services
{
    public class KafkaDlqService : IDeadLetterQueueService
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _dlqTopic;
        private readonly ILogger<KafkaDlqService> _logger;
        private bool _disposed;

        public KafkaDlqService(
            ProducerConfig config,
            IConfiguration configuration,
            ILogger<KafkaDlqService> logger)
        {
            _logger = logger;
            _dlqTopic = configuration["Kafka:Topics:DeadLetterQueue"] ?? "dead-letter-queue";

            _producer = new ProducerBuilder<string, string>(config)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
                .SetErrorHandler((_, e) => _logger.LogError("Kafka DLQ Producer Error: {Reason}", e.Reason))
                .Build();
        }

        public async Task SendToDlqAsync(string originalMessage, string error, CancellationToken ct)
        {
            if (_disposed)
            {
                _logger.LogWarning("Attempted to send to DLQ after service disposal.");
                return;
            }

            var dlqPayload = JsonSerializer.Serialize(new
            {
                OriginalMessage = originalMessage,
                Error = error,
                Timestamp = DateTime.UtcNow,
                Service = "NotificationService"
            });

            try
            {
                var result = await _producer.ProduceAsync(_dlqTopic, new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = dlqPayload
                }, ct);

                _logger.LogDebug(
                    "Message sent to DLQ topic {Topic} at offset {Offset}",
                    _dlqTopic, result.Offset);
            }
            catch(Exception ex)
            {
                _logger.LogError(
                    ex, 
                    "Failed to send message to DLQ topic {Topic}. Original error: {Error}",
                    _dlqTopic, error);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    _producer?.Flush(TimeSpan.FromSeconds(5));
                    _producer?.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during DLQ producer disposal.");
                }
                _disposed = true;
            }
        }
    }
}
