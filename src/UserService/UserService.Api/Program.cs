using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using UserService.Application;
using UserService.Application.Interfaces.Service;
using UserService.Application.Services;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IEventPublisher, KafkaEventPublisher>();

builder.Services.AddSingleton<IProducer<string, string>>(sp =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"],
        EnableIdempotence = true,
        Acks = Acks.All,
        LingerMs = 5,
        BatchNumMessages = 100
    };

    return new ProducerBuilder<string, string>(config)
    .SetErrorHandler((_, e) =>
    sp.GetRequiredService<ILogger<IProducer<string, string>>>().LogError("Kafka Producer Error: {Reason}", e.Reason))
    .Build();
}
           );

// === EF Core ===
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IFavoritesRepository, FavoritesRepository>();
builder.Services.AddScoped<IUserProfilesRepository, UserProfilesRepository>();

builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();

//    db.Database.Migrate();
//}

app.UseAuthorization();
app.MapControllers();

app.Run();