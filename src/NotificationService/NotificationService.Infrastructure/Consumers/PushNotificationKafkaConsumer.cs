using Confluent.Kafka;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Application.DTOs;
using NotificationService.Application.Services.Interface;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Repository.Interface;
using NotificationService.Infrastructure.Services.Interface;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace NotificationService.Infrastructure.Consumers
{
    public class PushNotificationKafkaConsumer : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly INotificationRepository _repository;
        private readonly IDeadLetterQueueService _dlqService;
        private readonly IEventNotificationFactory _notificationFactory;
        private readonly ILogger<PushNotificationKafkaConsumer> _logger;
        private readonly string[] _topics;
        private readonly JsonSerializerOptions _jsonOptions;

        public PushNotificationKafkaConsumer(
           ConsumerConfig config,
            INotificationRepository repository,
            IDeadLetterQueueService dlqService,
            IEventNotificationFactory notificationFactory,
            ILogger<PushNotificationKafkaConsumer> logger,
            IConfiguration configuration)
        {
            _topics = configuration.GetSection("Kafka:SubscribedTopics")
               .GetChildren()
               .Select(c => c.Value)
               .ToArray()
           ?? new[] { "user.events", "content.events" };

            config.GroupId = "notification-service-consumer";
            config.EnableAutoCommit = false;
            config.AutoOffsetReset = AutoOffsetReset.Earliest;
            config.BootstrapServers = config.BootstrapServers ?? "kafka:9092";

            _consumer = new ConsumerBuilder<string, string>(config)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(Deserializers.Utf8)
                .SetErrorHandler((_, e) => logger.LogError("Kafka Error: {Reason}", e.Reason))
                .Build();

            _repository = repository;
            _dlqService = dlqService;
            _notificationFactory = notificationFactory;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_topics);
            _logger.LogInformation("Consumer started. Subscribed to topics: {Topics}", string.Join(", ", _topics));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);
                    if (result?.Message == null) continue;

                    await ProcessMessageAsync(result, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    _logger.LogError(e, "Kafka consume error: {Error}", e.Error.Reason);
                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unexpected error in consumer loop");
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }

        private async Task ProcessMessageAsync(ConsumeResult<string, string> result, CancellationToken ct)
        {
            var value = result.Message.Value;
            var offset = result.TopicPartitionOffset;
            var topic = result.Topic;

            try
            {
                var notification = topic switch
                {
                    var t when t.StartsWith("user") => ParseUserEvent(value),
                    var t when t.StartsWith("content") => ParseContentEvent(value),
                    _ => null
                };

                if (notification == null)
                {
                    _logger.LogWarning("Unknown topic or invalid event type: {Topic}", topic);
                    await _dlqService.SendToDlqAsync(value, $"Unknown topic or invalid event type: {topic}", ct);
                    _consumer.Commit(result);
                    return;
                }

                await _repository.AddAsync(notification, ct);
                await _repository.AddLogAsync(new NotificationLog
                {
                    Id = Guid.NewGuid(),
                    NotificationId = notification.Id,
                    Provider = "Kafka",
                    Status = "Received",
                    ErrorMessage = null,
                    CreatedAt = DateTime.UtcNow
                }, ct);

                await _repository.SaveChangesAsync(ct);
                _consumer.Commit(result);

                _logger.LogInformation("Notification saved. UserId: {UserId}, Type: {Type}", notification.UserId, notification.Type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message at offset {Offset}", offset);
                await _dlqService.SendToDlqAsync(value, ex.Message, ct);
                _consumer.Commit(result);
            }
        }

        private Notification? ParseUserEvent(string value)
        {
            var dto = JsonSerializer.Deserialize<UserEventDto>(value, _jsonOptions);
            return dto == null ? null : _notificationFactory.CreateFromUserEvent(dto);
        }

        private Notification? ParseContentEvent(string value)
        {
            var dto = JsonSerializer.Deserialize<ContentEventDto>(value, _jsonOptions);
            return dto == null ? null : _notificationFactory.CreateFromContentEvent(dto);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Остановка consumer");
            _consumer.Close();
            await base.StopAsync(cancellationToken);
        }
    }
}
