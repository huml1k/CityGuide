using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Services;
using NotificationService.Application.Services.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNotificationApplication(this IServiceCollection services)
        {
            services.AddSingleton<IEventNotificationFactory, EventNotificationFactory>();
            return services;
        }
    }
}
