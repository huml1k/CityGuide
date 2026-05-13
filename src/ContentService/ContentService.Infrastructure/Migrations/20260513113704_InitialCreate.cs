using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ContentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "routes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "draft"),
                    google_maps_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_routes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "audio_files",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    route_id = table.Column<long>(type: "bigint", nullable: false),
                    file_extension = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    duration_seconds = table.Column<int>(type: "integer", nullable: true),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    original_filename = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audio_files", x => x.id);
                    table.ForeignKey(
                        name: "FK_audio_files_routes_route_id",
                        column: x => x.route_id,
                        principalTable: "routes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "route_images",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    route_id = table.Column<long>(type: "bigint", nullable: false),
                    file_extension = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    is_cover = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_route_images", x => x.id);
                    table.ForeignKey(
                        name: "FK_route_images_routes_route_id",
                        column: x => x.route_id,
                        principalTable: "routes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "route_points",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    route_id = table.Column<long>(type: "bigint", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_route_points", x => x.id);
                    table.ForeignKey(
                        name: "FK_route_points_routes_route_id",
                        column: x => x.route_id,
                        principalTable: "routes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "route_reviews",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    route_id = table.Column<long>(type: "bigint", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_route_reviews", x => x.id);
                    table.ForeignKey(
                        name: "FK_route_reviews_routes_route_id",
                        column: x => x.route_id,
                        principalTable: "routes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "route_stats",
                columns: table => new
                {
                    route_id = table.Column<long>(type: "bigint", nullable: false),
                    favorites_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    average_rating = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    reviews_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_route_stats", x => x.route_id);
                    table.ForeignKey(
                        name: "FK_route_stats_routes_route_id",
                        column: x => x.route_id,
                        principalTable: "routes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "route_tags",
                columns: table => new
                {
                    route_id = table.Column<long>(type: "bigint", nullable: false),
                    tag_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_route_tags", x => new { x.route_id, x.tag_id });
                    table.ForeignKey(
                        name: "FK_route_tags_routes_route_id",
                        column: x => x.route_id,
                        principalTable: "routes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_route_tags_tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_audio_progress",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    audio_file_id = table.Column<long>(type: "bigint", nullable: false),
                    progress_seconds = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_audio_progress", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_audio_progress_audio_files_audio_file_id",
                        column: x => x.audio_file_id,
                        principalTable: "audio_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_audio_files_route_id",
                table: "audio_files",
                column: "route_id");

            migrationBuilder.CreateIndex(
                name: "IX_audio_files_route_id_order_index",
                table: "audio_files",
                columns: new[] { "route_id", "order_index" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_route_images_route_id",
                table: "route_images",
                column: "route_id");

            migrationBuilder.CreateIndex(
                name: "IX_route_points_route_id",
                table: "route_points",
                column: "route_id");

            migrationBuilder.CreateIndex(
                name: "IX_route_reviews_route_id_user_id",
                table: "route_reviews",
                columns: new[] { "route_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_route_tags_tag_id",
                table: "route_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_routes_creator_id",
                table: "routes",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "IX_routes_deleted_at",
                table: "routes",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "IX_routes_status",
                table: "routes",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_tags_name",
                table: "tags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_audio_progress_audio_file_id",
                table: "user_audio_progress",
                column: "audio_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_audio_progress_user_id_audio_file_id",
                table: "user_audio_progress",
                columns: new[] { "user_id", "audio_file_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "route_images");

            migrationBuilder.DropTable(
                name: "route_points");

            migrationBuilder.DropTable(
                name: "route_reviews");

            migrationBuilder.DropTable(
                name: "route_stats");

            migrationBuilder.DropTable(
                name: "route_tags");

            migrationBuilder.DropTable(
                name: "user_audio_progress");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "audio_files");

            migrationBuilder.DropTable(
                name: "routes");
        }
    }
}
