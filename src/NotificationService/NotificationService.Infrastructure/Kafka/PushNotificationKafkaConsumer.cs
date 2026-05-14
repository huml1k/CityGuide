using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NotificationService.Infrastructure.Kafka
{
    public class PushNotificationKafkaConsumer : BackgroundService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IConfiguration _config;
        private readonly ILogger<PushNotificationKafkaConsumer> _logger;
      
        public PushNotificationKafkaConsumer(
            IConfiguration config,
            ILogger<PushNotificationKafkaConsumer> logger) 
        {
            _logger = logger;
            _config = config;

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _config["Kafka:BootstrapServers"],
                GroupId = _config["Kafka:GroupId"],
                AutoOffsetReset = _config["Kafka:AutoOffsetReset"] == "Latest"
                ? AutoOffsetReset.Latest
                : AutoOffsetReset.Earliest,
                MaxPollIntervalMs = 300_000,
                EnableAutoCommit = false
            };

            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }
    }
}
