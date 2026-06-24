using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anonwork.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovePostImageAndEmailVerificationToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "email_verification_tokens");

            migrationBuilder.DropTable(
                name: "post_images");

            migrationBuilder.RenameColumn(
                name: "avatar_url",
                table: "users",
                newName: "avatar_key");

            migrationBuilder.RenameColumn(
                name: "image_url",
                table: "anon_images",
                newName: "file_key");

            migrationBuilder.AddColumn<string>(
                name: "content_type",
                table: "anon_images",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "file_size",
                table: "anon_images",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "post_media",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    media_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    file_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    content_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    file_size = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    original_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("post_media_pkey", x => x.id);
                    table.ForeignKey(
                        name: "post_media_post_id_fkey",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_post_media_post_id",
                table: "post_media",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_media_post_id_display_order",
                table: "post_media",
                columns: new[] { "post_id", "display_order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "post_media");

            migrationBuilder.DropColumn(
                name: "content_type",
                table: "anon_images");

            migrationBuilder.DropColumn(
                name: "file_size",
                table: "anon_images");

            migrationBuilder.RenameColumn(
                name: "avatar_key",
                table: "users",
                newName: "avatar_url");

            migrationBuilder.RenameColumn(
                name: "file_key",
                table: "anon_images",
                newName: "image_url");

            migrationBuilder.CreateTable(
                name: "email_verification_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    token_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("email_verification_tokens_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "post_images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    display_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    image_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("post_images_pkey", x => x.id);
                    table.ForeignKey(
                        name: "post_images_post_id_fkey",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_email_verification_tokens_email",
                table: "email_verification_tokens",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_email_verification_tokens_token_hash",
                table: "email_verification_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_post_images_post_id",
                table: "post_images",
                column: "post_id");
        }
    }
}
