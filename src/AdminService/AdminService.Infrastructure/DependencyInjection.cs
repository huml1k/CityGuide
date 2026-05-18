using AdminService.Application.Interfaces;
using AdminService.Infrastructure.Health;
using Microsoft.Extensions.DependencyInjection;

namespace AdminService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAdminInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IDatabaseHealthChecker, DatabaseHealthChecker>();

        return services;
    }
}
