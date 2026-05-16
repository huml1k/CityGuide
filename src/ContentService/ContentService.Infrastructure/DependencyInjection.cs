using ContentService.Domain.Interfaces.Repositories;
using ContentService.Infrastructure.Extensions;
using ContentService.Infrastructure.Persistence;
using ContentService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ContentDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IRouteRepository, RouteRepository>();

            services.AddScoped<IAudioFileRepository, AudioFileRepository>();

            services.AddScoped<IRouteImageRepository, RouteImageRepository>();

            services.AddScoped<ITagRepository, TagRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Redis
            //MinIO

            services.AddKafkaProducer(configuration);

            return services;
        }
    }
}
