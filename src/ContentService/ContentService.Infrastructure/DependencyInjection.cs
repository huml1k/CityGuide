using ContentService.Domain.Interfaces.Repositories;
using ContentService.Infrastructure.Persistence;
using ContentService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddDbContext<ContentDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IRouteRepository, RouteRepository>();

            services.AddScoped<ITagRepository, TagRepository>();

            services.AddScoped<IRouteReviewRepository, RouteReviewRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Redis
            //MinIO
            //Kafka

            return services;
        }
    }
}
