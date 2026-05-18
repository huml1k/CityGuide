using AdminService.Application;
using AdminService.Application.Options;
using AdminService.Infrastructure;
using AdminService.Infrastructure.Persistence;
using AuthService.AuthKit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCityGuideServiceAuth(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<CityGuideServicesOptions>(
    builder.Configuration.GetSection(CityGuideServicesOptions.SectionName));

builder.Services.AddAdminApplication();
builder.Services.AddAdminInfrastructure();

// === EF Core ===
builder.Services.AddDbContext<AdminDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AdminDbContext>();

//    db.Database.Migrate();
//}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();