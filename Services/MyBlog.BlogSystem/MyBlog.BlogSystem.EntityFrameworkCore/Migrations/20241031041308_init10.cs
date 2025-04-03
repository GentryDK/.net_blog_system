using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBlog.BlogSystem.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class init10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishStatus",
                table: "Post");

            migrationBuilder.RenameColumn(
                name: "discuss",
                table: "Post",
                newName: "Discuss");

            migrationBuilder.AddColumn<DateTime>(
                name: "EditDate",
                table: "Post",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EditDate",
                table: "Post");

            migrationBuilder.RenameColumn(
                name: "Discuss",
                table: "Post",
                newName: "discuss");

            migrationBuilder.AddColumn<string>(
                name: "PublishStatus",
                table: "Post",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
