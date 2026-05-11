using UserService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace UserService.Infrastructure.Persistence;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==================== UserProfile ====================
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("user_profiles");
            entity.HasKey(e => e.UserId);           // 1:1 с User

            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.FullName).HasColumnName("full_name").HasMaxLength(150);
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(500);
            entity.Property(e => e.Bio).HasColumnName("bio").HasMaxLength(1000);
            entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(30);
        });

        // ==================== Favorite ====================
        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.ToTable("favorites");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.RouteId).HasColumnName("route_id").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // Индексы
            entity.HasIndex(e => new { e.UserId, e.RouteId }).IsUnique(); // один пользователь может добавить маршрут в избранное только один раз
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.RouteId);
            entity.HasIndex(e => e.CreatedAt);

        });
    }
}