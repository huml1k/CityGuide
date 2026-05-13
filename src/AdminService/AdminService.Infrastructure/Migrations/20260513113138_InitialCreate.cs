using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AdminService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admin_logs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    admin_id = table.Column<int>(type: "integer", nullable: false),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    entity_id = table.Column<long>(type: "bigint", nullable: true),
                    details = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "route_reports",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    route_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "pending"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    reviewed_by = table.Column<int>(type: "integer", nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_route_reports", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_moderation",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_moderation", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_logs");

            migrationBuilder.DropTable(
                name: "route_reports");

            migrationBuilder.DropTable(
                name: "user_moderation");
        }
    }
}
