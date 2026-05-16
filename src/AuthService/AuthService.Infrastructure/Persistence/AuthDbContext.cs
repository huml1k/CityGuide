using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence;

public sealed class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.ToTable("user_sessions");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id")
                  .IsRequired();

            entity.Property(e => e.RefreshTokenHash)
                  .HasColumnName("refresh_token_hash")
                  .HasMaxLength(255)
                  .IsRequired();

            entity.Property(e => e.ExpiresAt)
                  .HasColumnName("expires_at")
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .IsRequired();

            entity.Property(e => e.RevokedAt)
                  .HasColumnName("revoked_at");

            entity.Property(e => e.UserAgent)
                  .HasColumnName("user_agent")
                  .HasMaxLength(255);

            entity.Property(e => e.IpAddress)
                  .HasColumnName("ip_address")
                  .HasMaxLength(45);
        });

        // Глобальный query filter для soft delete (опционально, но рекомендуется)
        modelBuilder.Entity<User>()
            .HasQueryFilter(u => u.DeletedAt == null);
    }
}
