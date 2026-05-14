using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Persistence;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.Email)
                  .HasColumnName("email")
                  .HasMaxLength(255)
                  .IsRequired();

            entity.Property(e => e.PasswordHash)
                  .HasColumnName("password_hash")
                  .HasMaxLength(512)
                  .IsRequired();

            entity.Property(e => e.Role)
                  .HasColumnName("role")
                  .HasMaxLength(50)
                  .HasDefaultValue("User")
                  .IsRequired();

            entity.Property(e => e.IsEmailConfirmed)
                  .HasColumnName("is_email_confirmed")
                  .HasDefaultValue(false)
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .IsRequired();

            entity.Property(e => e.UpdatedAt)
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");

            // Индексы
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.DeletedAt); // для soft delete фильтров
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

            // Связь с User
            entity.HasOne(x => x.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade); // или Restrict, в зависимости от политики
        });

        // Глобальный query filter для soft delete (опционально, но рекомендуется)
        modelBuilder.Entity<User>()
            .HasQueryFilter(u => u.DeletedAt == null);
    }
}