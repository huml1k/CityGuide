using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Interfaces
{
    public interface IKafkaEventPublisher
    {
        public Task PublishAsync<T>(string topic, T @event, CancellationToken ct = default);
    }
}
