using System;
using System.Collections.Generic;
using System.Text;
using ContentService.Application.Interfaces;
using ContentService.Infrastructure.MinIo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;

namespace ContentService.Infrastructure.Extensions
{
    public static class MinioExtensions
    {
        public static IServiceCollection AddMinIoStorage(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<MinIoOptions>()
                .Bind(configuration.GetSection(MinIoOptions.SectionName))
                .Validate(o => !string.IsNullOrWhiteSpace(o.Endpoint), "MinIo:Endpoint is required")
                .Validate(o => !string.IsNullOrWhiteSpace(o.ContentBucket), "MinIo:ContentBucket is required")
                .ValidateOnStart();

            services.AddSingleton<IMinioClient>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MinIoOptions>>().Value;
                var builder = new MinioClient()
                    .WithEndpoint(options.Endpoint)
                    .WithCredentials(options.AccessKey, options.SecretKey);
                if (options.UseSsl)
                    builder = builder.WithSSL();
                return builder.Build();
            });

            services.AddScoped<IFileStorageService, MinIoFileStorage>();
            return services;
        }
    }
}
