//using Confluent.Kafka;
//using Mapster;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using NotificationService.Application.DTOs;
//using NotificationService.Domain.Entities;
//using NotificationService.Infrastructure.Repository.Interface;
//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Text.Json;

//namespace NotificationService.Infrastructure.Consumers
//{
//    public class PushNotificationKafkaConsumer : BackgroundService
//    {
//        private readonly IConsumer<string, string> _consumer;
//        private readonly IConfiguration _config;
//        private readonly ILogger<PushNotificationKafkaConsumer> _logger;
//        private readonly INotificationRepository _repository;
//        private readonly string _topic;

//        public PushNotificationKafkaConsumer(
//            IConfiguration config,
//            ILogger<PushNotificationKafkaConsumer> logger,
//            INotificationRepository repository)
//        {
//            _topic = config["Kafka:Topics:PushNotifications"] ?? "push-notifications";
//            _logger = logger;
//            _config = config;
//            _repository = repository;

//            var consumerConfig = new ConsumerConfig
//            {
//                BootstrapServers = _config["Kafka:BootstrapServers"],
//                GroupId = _config["Kafka:GroupId"],
//                AutoOffsetReset = _config["Kafka:AutoOffsetReset"] == "Latest"
//                ? AutoOffsetReset.Latest
//                : AutoOffsetReset.Earliest,
//                MaxPollIntervalMs = 300_000,
//                EnableAutoCommit = false
//            };

//            _consumer = new ConsumerBuilder<string, string>(consumerConfig)
//                .SetKeyDeserializer(Deserializers.Utf8)
//                .SetValueDeserializer(Deserializers.Utf8)
//                .SetErrorHandler((_, e) => logger.LogError("Ошибка Kafka: {Reason}", e.Reason))
//                .Build();
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            _consumer.Subscribe(_topic);

//            while (!stoppingToken.IsCancellationRequested)
//            {
//                try
//                {
//                    var result = _consumer.Consume(stoppingToken);
//                    if (result?.Message == null) continue;

//                    await ProccessMessageAsync(result, stoppingToken);
//                }
//                catch (OperationCanceledException)
//                {
//                    break;
//                }
//                catch (ConsumeException e)
//                {
//                    _logger.LogError(e, "Kafka consume Ошибка: {Error}", e.Error.Reason);
//                    await Task.Delay(1000, stoppingToken);
//                }
//                catch (Exception e)
//                {
//                    _logger.LogError(e, "Неопознаная ошибка в consumer loop");
//                    await Task.Delay(1000, stoppingToken);
//                }

//            }
//        }

//        private async Task ProccessMessageAsync(ConsumeResult<string, string> result, CancellationToken ct)
//        {
//            var value = result.Message.Value;
//            var offset = result.TopicPartitionOffset;

//            try
//            {
//                var dto = JsonSerializer.Deserialize<KafkaPushNotificationDto>(value, new JsonSerializerOptions
//                {
//                    PropertyNameCaseInsensitive = true
//                });

//                if (dto == null)
//                {
//                    _logger.LogError("Не удалось десериализовать по offset {Offset}", offset);
//                    //await _dlqService.SendToDlqAsync(value, "Deserialization failed", ct);
//                    _consumer.Commit(result);
//                    return;
//                }

//                var notification = dto.Adapt<Notification>();

//                await _repository.AddAsync(notification, ct);

//                var log = new NotificationLog
//                {
//                    Id = Guid.NewGuid(),
//                    NotificationId = notification.Id,
//                    Provider = "Kafka",
//                    Status = "Received",
//                    ErrorMessage = null,
//                    CreatedAt = DateTime.UtcNow
//                };
//                await _repository.AddLogAsync(log, ct);

//                await _repository.SaveChangesAsync(ct);

//                _consumer.Commit(result);
//                _logger.LogInformation("Уведомление успешно сохранено. UserId: {UserId}, Id: {Id}",
//                    notification.UserId, notification.Id);
//            }
//            catch (Exception e)
//            {
//                _logger.LogError(e, "Ошибка обработки сообщении в offset {Offset}", offset);

//                _consumer.Commit(result);
//            }
//        }

//        public override async Task StopAsync(CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Остановка consumer");
//            _consumer.Close();
//            await base.StopAsync(cancellationToken);
//        }
//    }
//}
