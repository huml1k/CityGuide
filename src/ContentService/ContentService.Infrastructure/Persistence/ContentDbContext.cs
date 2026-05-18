using ContentService.Domain.Entities;
using ContentService.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ContentService.Infrastructure.Persistence;

public class ContentDbContext : DbContext
{
    public ContentDbContext(DbContextOptions<ContentDbContext> options)
        : base(options)
    {
    }

    public DbSet<AudioFile> AudioFiles => Set<AudioFile>();
    public DbSet<Route> Routes => Set<Route>();
    public DbSet<RouteImage> RouteImages => Set<RouteImage>();
    public DbSet<RoutePoint> RoutePoints => Set<RoutePoint>();
    public DbSet<RouteStats> RouteStats => Set<RouteStats>();
    public DbSet<RouteTag> RouteTags => Set<RouteTag>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==================== AudioFile ====================
        modelBuilder.Entity<AudioFile>(entity =>
        {
            entity.ToTable("audio_files");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RouteId).HasColumnName("route_id").IsRequired();
            entity.Property(e => e.FileExtension).HasColumnName("file_extension").HasMaxLength(5).IsRequired();
            entity.Property(e => e.DurationSeconds).HasColumnName("duration_seconds");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index").IsRequired();
            entity.Property(e => e.OriginalFilename).HasColumnName("original_filename").HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

            // Связь с Route (один маршрут — много аудиофайлов)
            entity.HasOne(x => x.Route)
                .WithMany(x => x.AudioFiles)
                .HasForeignKey(x => x.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.RouteId);
            entity.HasIndex(e => new { e.RouteId, e.OrderIndex }).IsUnique(); // порядок файлов в маршруте
        });

        // ==================== Route ====================
        modelBuilder.Entity<Route>(entity =>
        {
            entity.ToTable("routes");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatorId).HasColumnName("creator_id").IsRequired();
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(2000);
            entity.Property(e => e.DurationMinutes).HasColumnName("duration_minutes").IsRequired();
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(50)
                .HasConversion<string>()
                .HasDefaultValueSql("'pendingModeration'")
                .IsRequired();
            entity.Property(e => e.GoogleMapsUrl).HasColumnName("google_maps_url").HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatorId);
            entity.HasIndex(e => e.DeletedAt);

            // Soft Delete
            entity.HasQueryFilter(r => r.DeletedAt == null);
        });

        // ==================== RouteImage ====================
        modelBuilder.Entity<RouteImage>(entity =>
        {
            entity.ToTable("route_images");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RouteId).HasColumnName("route_id").IsRequired();
            entity.Property(e => e.FileExtension).HasColumnName("file_extension").HasMaxLength(5).IsRequired();
            entity.Property(e => e.IsCover).HasColumnName("is_cover").HasDefaultValue(false).IsRequired();
            entity.Property(e => e.OrderIndex).HasColumnName("order_index").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

            entity.HasOne(x => x.Route)
            .WithMany(x => x.RouteImages)
            .HasForeignKey(x => x.RouteId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        // ==================== RoutePoint ====================
        modelBuilder.Entity<RoutePoint>(entity =>
        {
            entity.ToTable("route_points");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RouteId).HasColumnName("route_id").IsRequired();
            entity.Property(e => e.Latitude).HasColumnName("latitude").IsRequired();
            entity.Property(e => e.Longitude).HasColumnName("longitude").IsRequired();
            entity.Property(e => e.OrderIndex).HasColumnName("order_index").IsRequired();
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(150);
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);

            entity.HasOne(x => x.Route)
            .WithMany(x => x.RoutePoints)
            .HasForeignKey(x => x.RouteId)
            .OnDelete(DeleteBehavior.Cascade);
        });


        // ==================== RouteStats ====================
        modelBuilder.Entity<RouteStats>(entity =>
        {
            entity.ToTable("route_stats");
            entity.HasKey(e => e.RouteId); // 1:1

            entity.Property(e => e.RouteId).HasColumnName("route_id");
            entity.Property(e => e.FavoritesCount).HasColumnName("favorites_count").HasDefaultValue(0);
            entity.HasOne(x => x.Route)
                  .WithOne(x => x.RouteStats)
                  .HasForeignKey<RouteStats>(x => x.RouteId)
                  .OnDelete(DeleteBehavior.Cascade);




        });

        // ==================== Tag ====================
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("tags");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

            entity.HasIndex(e => e.Name).IsUnique();
        });

        // ==================== RouteTag (Many-to-Many) ====================
        modelBuilder.Entity<RouteTag>(entity =>
        {
            entity.ToTable("route_tags");
            entity.HasKey(e => new { e.RouteId, e.TagId });

            entity.Property(e => e.RouteId).HasColumnName("route_id").IsRequired();
            entity.Property(e => e.TagId).HasColumnName("tag_id").IsRequired();

            entity.HasOne(x => x.Route)
                .WithMany(x => x.RouteTags)
                .HasForeignKey(x => x.RouteId);

            entity.HasOne(x => x.Tag)
                .WithMany(x => x.RouteTags)
                .HasForeignKey(x => x.TagId);
        });

    }
}