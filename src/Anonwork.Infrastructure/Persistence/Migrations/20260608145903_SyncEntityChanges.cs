using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anonwork.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncEntityChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_orders_expires_at",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "comments");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "subjects",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "subjects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "reports",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "posts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValueSql: "'active'::character varying");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "orders",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "payment_method",
                table: "orders",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "messages",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "messages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "conversations",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "comments",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "ix_orders_expires_at",
                table: "orders",
                column: "expires_at",
                filter: "status = 'Pending'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_orders_expires_at",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "subjects");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "subjects");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "comments");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "reports",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "posts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValueSql: "'active'::character varying",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Pending");

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "payment_method",
                table: "orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "comments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "comments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_orders_expires_at",
                table: "orders",
                column: "expires_at",
                filter: "status = 0");
        }
    }
}
