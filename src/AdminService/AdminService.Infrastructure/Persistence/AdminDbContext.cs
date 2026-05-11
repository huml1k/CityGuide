using AdminService.Domain.Entities;  
using Microsoft.EntityFrameworkCore;

namespace AdminService.Infrastructure.Persistence;

public class AdminDbContext : DbContext
{
    public AdminDbContext(DbContextOptions<AdminDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<AdminLog> AdminLogs => Set<AdminLog>();
    public DbSet<RouteReport> RouteReports => Set<RouteReport>();
    public DbSet<UserModeration> UserModerations => Set<UserModeration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AdminDbContext).Assembly);

        // AdminLog
        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.ToTable("admin_logs");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Action)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.EntityType)
                  .HasMaxLength(100);

            entity.Property(e => e.IpAddress)
                  .HasMaxLength(45);

            entity.Property(e => e.Details)
                  .HasColumnType("jsonb");
        });

        // RouteReport
        modelBuilder.Entity<RouteReport>(entity =>
        {
            entity.ToTable("route_reports");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Reason)
                  .IsRequired()
                  .HasMaxLength(255);

            entity.Property(e => e.Comment);

            entity.Property(e => e.Status)
                  .IsRequired()
                  .HasMaxLength(20)
                  .HasDefaultValue("pending");
        });

        // UserModeration
        modelBuilder.Entity<UserModeration>(entity =>
        {
            entity.ToTable("user_moderation");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Action)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.Reason)
                  .HasMaxLength(255);
        });
    }
}