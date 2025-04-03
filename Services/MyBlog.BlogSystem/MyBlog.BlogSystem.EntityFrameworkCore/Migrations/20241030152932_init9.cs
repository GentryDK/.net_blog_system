using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBlog.BlogSystem.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class init9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublishStatus",
                table: "Post",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "discuss",
                table: "Post",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishStatus",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "discuss",
                table: "Post");
        }
    }
}
