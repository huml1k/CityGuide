using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.AuthKit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCityGuideServiceAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CityGuideAuthOptions>(configuration.GetSection(CityGuideAuthOptions.SectionName));

        var options = configuration.GetSection(CityGuideAuthOptions.SectionName).Get<CityGuideAuthOptions>()
                      ?? throw new InvalidOperationException($"Section '{CityGuideAuthOptions.SectionName}' is missing.");
        ValidateOptions(options);

        return services.AddCityGuideServiceAuth(options);
    }

    public static IServiceCollection AddCityGuideServiceAuth(
        this IServiceCollection services,
        Action<CityGuideAuthOptions> configure)
    {
        var options = new CityGuideAuthOptions();
        configure(options);
        ValidateOptions(options);

        return services.AddCityGuideServiceAuth(options);
    }

    private static IServiceCollection AddCityGuideServiceAuth(
        this IServiceCollection services,
        CityGuideAuthOptions options)
    {
        services.AddSingleton(options);
        services.AddSingleton<IOptions<CityGuideAuthOptions>>(_ => Options.Create(options));
        services.AddAuthorization();
        services.AddHttpContextAccessor();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.JwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = options.JwtAudience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.JwtSecret)),
                    ValidateLifetime = options.ValidateLifetime,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        return services;
    }

    private static void ValidateOptions(CityGuideAuthOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.JwtSecret) || options.JwtSecret.Length < 32)
        {
            throw new InvalidOperationException("CityGuideAuth:JwtSecret must be at least 32 chars.");
        }
    }
}
