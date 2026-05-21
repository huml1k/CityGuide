using System.Threading.RateLimiting;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddCors(cors =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
    cors.AddPolicy("AllowFrontend", policy => policy
        .WithOrigins(origins ?? Array.Empty<string>())
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync(
            "Too many requests. Please try again later.", token);
    };

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:Global:PermitLimit", 100),
                Window = TimeSpan.Parse(builder.Configuration.GetValue<string>("RateLimiting:Global:Window", "00:01:00")),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = builder.Configuration.GetValue<int>("RateLimiting:Global:QueueLimit", 0)
            }));

    options.AddPolicy("AuthLimit", ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:Auth:PermitLimit", 10),
                Window = TimeSpan.Parse(builder.Configuration.GetValue<string>("RateLimiting:Auth:Window", "00:01:00")),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transformBuilder => 
    {
        transformBuilder.AddRequestTransform(async transformContext =>
        {
            var logger = transformContext.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var correlationId = Guid.NewGuid().ToString("N");

            logger.LogInformation("Proxy: {Method} {Path} [CorrelationId: {CorrelationId}]",
                transformContext.HttpContext.Request.Method,
                transformContext.HttpContext.Request.Path,
                correlationId);

            await Task.CompletedTask;
        });


    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Incoming request: {Method} {Path} from {IP}",
        context.Request.Method,
        context.Request.Path,
        context.Connection.RemoteIpAddress);

    await next();

    logger.LogInformation("Out request: {Method} {Path} -> {StatusCode}",
        context.Request.Method,
        context.Request.Path,
        context.Response.StatusCode);
});


app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseCors("AllowFrontend");


app.MapReverseProxy();

app.Run();


