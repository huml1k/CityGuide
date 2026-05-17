using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.Interfaces.Service
{
    public interface IEventPublisher
    {
        public Task PublishAsync<T>(string topic, T @event, CancellationToken ct = default);
    }
}
