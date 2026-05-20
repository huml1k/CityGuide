using AuthService.AuthKit;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using NotificationService.Api.Middleware;
using Microsoft.Extensions.Options;
using NotificationService.Application;
using NotificationService.Application.Services;
using NotificationService.Application.Services.Interface;
using NotificationService.Infrastructure;
using NotificationService.Infrastructure.Consumers;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.Repository;
using NotificationService.Infrastructure.Repository.Interface;
using NotificationService.Infrastructure.Services;
using NotificationService.Infrastructure.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCityGuideServiceAuth(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithBearerAuth();

builder.Services.AddNotificationApplication();
builder.Services.AddNotificationInfrastructure(builder.Configuration);

// === EF Core ===
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

    db.Database.Migrate();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();