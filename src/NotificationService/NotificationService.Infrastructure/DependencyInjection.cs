using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NotificationService.Infrastructure.Consumers;
using NotificationService.Infrastructure.Repository;
using NotificationService.Infrastructure.Repository.Interface;
using NotificationService.Infrastructure.Services;
using NotificationService.Infrastructure.Services.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNotificationInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ConsumerConfig>(cfg =>
            {
                cfg.BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "kafka:9092";
            });
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<ConsumerConfig>>().Value);

            services.Configure<ProducerConfig>(cfg =>
            {
                cfg.BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "kafka:9092";
            });
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<ProducerConfig>>().Value);

            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddSingleton<IDeadLetterQueueService, KafkaDlqService>();
            services.AddHostedService<PushNotificationKafkaConsumer>();

            return services;
        }
    }
}
