using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anonwork.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOneTimeTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "one_time_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    purpose = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    token_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("one_time_tokens_pkey", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_one_time_tokens_email",
                table: "one_time_tokens",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_one_time_tokens_email_purpose",
                table: "one_time_tokens",
                columns: new[] { "email", "purpose" });

            migrationBuilder.CreateIndex(
                name: "ix_one_time_tokens_token_hash",
                table: "one_time_tokens",
                column: "token_hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "one_time_tokens");
        }
    }
}
