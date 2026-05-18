using System.Text;
using AuthService.AuthKit.Internal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
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
                    ClockSkew = TimeSpan.Zero
                };

                jwt.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.Principal?.Identity is System.Security.Claims.ClaimsIdentity identity)
                        {
                            var authorization = context.HttpContext.Request.Headers.Authorization.ToString();
                            var accessToken = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                                ? authorization["Bearer ".Length..].Trim()
                                : null;

                            if (!string.IsNullOrWhiteSpace(accessToken))
                            {
                                identity.AddClaim(new System.Security.Claims.Claim("raw_access_token", accessToken));
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddHttpContextAccessor();
        services.AddHttpClient<AuthIntrospectionClient>((provider, client) =>
        {
            var settings = provider.GetRequiredService<CityGuideAuthOptions>();
            client.BaseAddress = new Uri(settings.IntrospectionBaseUrl);
        });

        services.AddTransient<Microsoft.AspNetCore.Authentication.IClaimsTransformation, IntrospectionClaimsTransformation>();

        return services;
    }

    private static void ValidateOptions(CityGuideAuthOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.JwtSecret) || options.JwtSecret.Length < 32)
        {
            throw new InvalidOperationException("CityGuideAuth:JwtSecret must be at least 32 chars.");
        }

        if (string.IsNullOrWhiteSpace(options.IntrospectionBaseUrl))
        {
            throw new InvalidOperationException("CityGuideAuth:IntrospectionBaseUrl is required.");
        }
    }
}

