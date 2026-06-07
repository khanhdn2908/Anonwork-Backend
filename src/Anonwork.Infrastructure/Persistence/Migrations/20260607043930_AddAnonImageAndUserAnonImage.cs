using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anonwork.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnonImageAndUserAnonImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "anon_image_id",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "anon_images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("anon_images_pkey", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_users_anon_image_id",
                table: "users",
                column: "anon_image_id");

            migrationBuilder.CreateIndex(
                name: "ix_anon_images_is_active",
                table: "anon_images",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_anon_images_name",
                table: "anon_images",
                column: "name");

            migrationBuilder.AddForeignKey(
                name: "users_anon_image_id_fkey",
                table: "users",
                column: "anon_image_id",
                principalTable: "anon_images",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "users_anon_image_id_fkey",
                table: "users");

            migrationBuilder.DropTable(
                name: "anon_images");

            migrationBuilder.DropIndex(
                name: "ix_users_anon_image_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "anon_image_id",
                table: "users");
        }
    }
}
