using Confluent.Kafka;
using ContentService.Application.Interfaces;
using ContentService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Infrastructure.Extensions
{
    public static class KafkaExtensions
    {
        public static IServiceCollection AddKafkaProducer(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IProducer<string, string>>(sp =>
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = configuration["Kafka:BootstrapServers"],
                    EnableIdempotence = true,
                    Acks = Acks.All,
                    LingerMs = 5,
                    BatchNumMessages = 100
                };

                return new ProducerBuilder<string, string>(config)
                .SetErrorHandler((_, e) =>
                sp.GetRequiredService<ILogger<IProducer<string, string>>>().LogError("Kafka Producer Error: {Reason}", e.Reason))
                .Build();
            }
            );

            services.AddSingleton<IKafkaEventPublisher, KafkaEventPublisher>();
            return services;
        }
    }
}
