using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Infrastructure.Services.Interface
{
    public interface IDeadLetterQueueService
    {
        Task SendToDlqAsync(string originalMessage, string error, CancellationToken ct);
    }
}
