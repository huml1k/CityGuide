using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Interfaces.Clients;
using UserService.Infrastructure.Clients;

namespace UserService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            //HTTP CLIENT

            services.AddHttpClient<IContentServiceClient, ContentServiceClient>(
                client =>
                {
                    client.BaseAddress =
                        new Uri(configuration["Services:ContentServiceUrl"]!);
                });

            return services;
        }
    }
}
