using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace UserService.Infrastructure.Persistence;

/// <summary>
/// Applies EF migrations for user_db. Handles legacy schema created by AuthService.EnsureCreated
/// (user_profiles without __EFMigrationsHistory).
/// </summary>
public static class UserDbMigrationRunner
{
    private const string InitialMigrationId = "20260518162544_Initial";

    public static void Migrate(UserDbContext db)
    {
        if (!db.Database.CanConnect())
        {
            throw new InvalidOperationException("Cannot connect to the UserService database.");
        }

        var applied = db.Database.GetAppliedMigrations().ToHashSet(StringComparer.Ordinal);
        if (!applied.Contains(InitialMigrationId) && TableExists(db, "user_profiles"))
        {
            EnsureFavoritesSchema(db);
            BaselineMigration(db, InitialMigrationId);
        }

        db.Database.Migrate();
    }

    private static void EnsureFavoritesSchema(UserDbContext db)
    {
        if (TableExists(db, "favorites"))
        {
            return;
        }

        db.Database.ExecuteSqlRaw(
            """
            CREATE TABLE favorites (
                id uuid NOT NULL,
                user_id uuid NOT NULL,
                route_id uuid NOT NULL,
                created_at timestamp with time zone NOT NULL,
                CONSTRAINT "PK_favorites" PRIMARY KEY (id)
            );
            CREATE INDEX "IX_favorites_created_at" ON favorites (created_at);
            CREATE INDEX "IX_favorites_route_id" ON favorites (route_id);
            CREATE INDEX "IX_favorites_user_id" ON favorites (user_id);
            CREATE UNIQUE INDEX "IX_favorites_user_id_route_id" ON favorites (user_id, route_id);
            """);
    }

    private static void BaselineMigration(UserDbContext db, string migrationId)
    {
        db.Database.ExecuteSqlRaw(
            """
            CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                "MigrationId" character varying(150) NOT NULL,
                "ProductVersion" character varying(32) NOT NULL,
                CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
            );
            """);

        var productVersion = ProductInfo.GetVersion();
        db.Database.ExecuteSqlRaw(
            """
            INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            VALUES ({0}, {1})
            ON CONFLICT ("MigrationId") DO NOTHING;
            """,
            migrationId,
            productVersion);
    }

    private static bool TableExists(DbContext db, string tableName)
    {
        var connection = db.Database.GetDbConnection();
        var shouldClose = connection.State != System.Data.ConnectionState.Open;
        if (shouldClose)
        {
            connection.Open();
        }

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'public' AND table_name = @tableName
                );
                """;
            var parameter = command.CreateParameter();
            parameter.ParameterName = "tableName";
            parameter.Value = tableName;
            command.Parameters.Add(parameter);

            return Convert.ToBoolean(command.ExecuteScalar());
        }
        finally
        {
            if (shouldClose)
            {
                connection.Close();
            }
        }
    }
}
