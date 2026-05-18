using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AuthService.AuthKit;

public static class SwaggerJwtBearerExtensions
{
    public const string SecuritySchemeId = "bearer";

    public static IServiceCollection AddSwaggerGenWithBearerAuth(
        this IServiceCollection services,
        Action<SwaggerGenOptions>? configure = null)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(SecuritySchemeId, new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. " +
                    "Example: \"Bearer {your_token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(SecuritySchemeId, document)] = [],
            });

            configure?.Invoke(options);
        });

        return services;
    }
}
