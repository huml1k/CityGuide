using System.Text;
using AuthService.Application;
using AuthService.Application.Models;
using AuthService.AuthKit;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithBearerAuth();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthorization();

var authOptions = builder.Configuration.GetSection(AuthOptions.SectionName).Get<AuthOptions>()
                 ?? throw new InvalidOperationException("Auth options are missing.");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.JwtSecret));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authOptions.JwtIssuer,
            ValidateAudience = true,
            ValidAudience = authOptions.JwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var authDbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    var userDbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();

    // AuthDbContext: только миграции (не смешивать с EnsureCreated — иначе 42P07 "already exists").
    await authDbContext.Database.MigrateAsync();

    // user_db: схему создаёт UserService (EF migrations). AuthService только использует user_profiles.
    if (!await userDbContext.Database.CanConnectAsync())
    {
        throw new InvalidOperationException("Cannot connect to user_db. Start UserService first or check ConnectionStrings:UserPostgres.");
    }
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
