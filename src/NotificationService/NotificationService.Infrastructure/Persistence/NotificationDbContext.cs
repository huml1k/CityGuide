using NotificationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace NotificationService.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==================== Notification ====================
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("notifications");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.Type).HasColumnName("type").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Message).HasColumnName("message").IsRequired();
            entity.Property(e => e.RelatedRouteId).HasColumnName("related_route_id");
            entity.Property(e => e.IsRead).HasColumnName("is_read").HasDefaultValue(false).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.UserId, e.IsRead }); // для получения непрочитанных
        });

        // ==================== NotificationLog ====================
        modelBuilder.Entity<NotificationLog>(entity =>
        {
            entity.ToTable("notification_logs");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NotificationId).HasColumnName("notification_id").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Provider).HasColumnName("provider").HasMaxLength(50).IsRequired();
            entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            entity.HasIndex(e => e.NotificationId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.NotificationId, e.Status });

            entity.HasOne(x => x.Notification)
            .WithMany()
            .HasForeignKey(e => e.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);

        });

    }
}
