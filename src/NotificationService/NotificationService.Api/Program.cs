using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NotificationService.Application.Services;
using NotificationService.Application.Services.Interface;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.Repository;
using NotificationService.Infrastructure.Repository.Interface;
using NotificationService.Infrastructure.Services;
using NotificationService.Infrastructure.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ConsumerConfig>(cfg =>
{
    cfg.BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "kafka:9092";
});
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<ConsumerConfig>>().Value);

builder.Services.Configure<ProducerConfig>(cfg =>
{
    cfg.BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "kafka:9092";
});
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<ProducerConfig>>().Value);


builder.Services.AddSingleton<IDeadLetterQueueService, KafkaDlqService>();
builder.Services.AddSingleton<IEventNotificationFactory, EventNotificationFactory>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// === EF Core ===
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddHostedService<PushNotificationKafkaConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

//    db.Database.Migrate();
//}

app.UseAuthorization();
app.MapControllers();

app.Run();