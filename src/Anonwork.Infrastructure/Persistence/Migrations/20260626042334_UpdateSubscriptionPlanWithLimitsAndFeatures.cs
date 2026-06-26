using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anonwork.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubscriptionPlanWithLimitsAndFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "features",
                table: "subscription_plans");

            migrationBuilder.AlterColumn<string>(
                name: "slug",
                table: "subscription_plans",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<bool>(
                name: "can_attach_media_to_post",
                table: "subscription_plans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "can_upload_post_files",
                table: "subscription_plans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "can_use_exclusive_anon_images",
                table: "subscription_plans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "can_use_premium_features",
                table: "subscription_plans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "subscription_plans",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "max_post_file_size_mb",
                table: "subscription_plans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "max_post_image_count",
                table: "subscription_plans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "max_post_media_count",
                table: "subscription_plans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "max_posts_per_day",
                table: "subscription_plans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "max_uploads_per_day",
                table: "subscription_plans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "subscription_plans",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "file_size",
                table: "anon_images",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "content_type",
                table: "anon_images",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_exclusive",
                table: "anon_images",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_anon_images_is_exclusive",
                table: "anon_images",
                column: "is_exclusive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_anon_images_is_exclusive",
                table: "anon_images");

            migrationBuilder.DropColumn(
                name: "can_attach_media_to_post",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "can_upload_post_files",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "can_use_exclusive_anon_images",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "can_use_premium_features",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "description",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "max_post_file_size_mb",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "max_post_image_count",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "max_post_media_count",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "max_posts_per_day",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "max_uploads_per_day",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "is_exclusive",
                table: "anon_images");

            migrationBuilder.AlterColumn<string>(
                name: "slug",
                table: "subscription_plans",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "features",
                table: "subscription_plans",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "file_size",
                table: "anon_images",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "content_type",
                table: "anon_images",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
