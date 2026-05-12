using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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
      
        public PushNotificationKafkaConsumer(
            IConfiguration config) 
        {
            _config = config;

            var bootstrapServers = _config["Kafka:BootstrapServers"];
            var groupId = _config["Kafka:GroupId"];
            var autoOffsetReset = _config["Kafka:AutoOffsetReset"] == "Latest"
                ? AutoOffsetReset.Latest
                : AutoOffsetReset.Earliest;
            var maxPollIntervalMs = 300_000;

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = autoOffsetReset,
                MaxPollIntervalMs = maxPollIntervalMs,
                EnableAutoCommit = true 
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
