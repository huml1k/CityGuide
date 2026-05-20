using AuthService.AuthKit;
using ContentService.Api.Middleware;
using ContentService.Application;
using ContentService.Application.Features.Routes.Commands.CreateRoute;
using ContentService.Infrastructure;
using ContentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCityGuideServiceAuth(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithBearerAuth();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContentDbContext>();

    db.Database.Migrate();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();