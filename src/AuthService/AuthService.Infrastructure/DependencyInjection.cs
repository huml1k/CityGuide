using AuthService.Application.Abstractions;
using AuthService.Application.Models;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Repositories;
using AuthService.Infrastructure.Security;
using AuthService.Infrastructure.Sessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.SectionName));

        var postgresConnection = configuration.GetConnectionString("Postgres")
                                 ?? throw new InvalidOperationException("ConnectionStrings:Postgres is required.");
        var userPostgresConnection = configuration.GetConnectionString("UserPostgres")
                                     ?? throw new InvalidOperationException("ConnectionStrings:UserPostgres is required.");
        var redisConnection = configuration.GetConnectionString("Redis")
                              ?? throw new InvalidOperationException("ConnectionStrings:Redis is required.");

        services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(postgresConnection));
        services.AddDbContext<UserDbContext>(options => options.UseNpgsql(userPostgresConnection));
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<ISessionStore, RedisSessionStore>();

        return services;
    }
}

