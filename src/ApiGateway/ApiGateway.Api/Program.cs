using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();



builder.Services.AddAuthorization(options => 
{
    options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddCors(cors =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
    cors.AddPolicy("AllowFrontend", policy => policy
        .WithOrigins(origins ?? Array.Empty<string>())
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transformBuilder => 
    {
        transformBuilder.AddRequestTransform(async transformContext =>
        {
            var logger = transformContext.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var correlationId = Guid.NewGuid().ToString("N");

            // Добавляем Correlation ID для сквозной трассировки
            transformContext.ProxyRequest.Headers.Remove("X-Correlation-Id");
            transformContext.ProxyRequest.Headers.TryAddWithoutValidation("X-Correlation-Id", correlationId);

            // Извлекаем UserId из JWT (claim "sub")
            var userId = transformContext.HttpContext.User.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                transformContext.ProxyRequest.Headers.Remove("X-User-Id");
                transformContext.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Id", userId);
            }

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

app.UseCors("AllowFrontend");

//app.UseAuthentication();
//app.UseAuthorization();


app.MapReverseProxy();

app.Run();


