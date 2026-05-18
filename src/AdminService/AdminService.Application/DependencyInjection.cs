using AdminService.Application.Interfaces;
using AdminService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AdminService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAdminApplication(this IServiceCollection services)
    {
        services.AddHttpClient<IHealthCheckService, HealthCheckService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        return services;
    }
}
