using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using NotificationService.Infrastructure.Services.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Infrastructure.Services
{
    public class KafkaDlqService 
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _dlqTopic;

        public KafkaDlqService(IConfiguration configuration)
        {
            _dlqTopic = configuration["Kafka:Topics:DeadLetterQueue"] ?? "dead-letter-queue";
            
        }
    }
}
