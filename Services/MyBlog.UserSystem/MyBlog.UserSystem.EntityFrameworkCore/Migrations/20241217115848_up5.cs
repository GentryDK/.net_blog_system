using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBlog.UserSystem.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class up5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Introduction",
                table: "User",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Introduction",
                table: "User");
        }
    }
}
