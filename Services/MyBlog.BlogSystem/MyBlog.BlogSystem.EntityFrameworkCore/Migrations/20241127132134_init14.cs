using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBlog.BlogSystem.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class init14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_PostType_PostTypeId",
                table: "Post");

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "Post",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Post",
                keyColumn: "PostTypeId",
                keyValue: null,
                column: "PostTypeId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "PostTypeId",
                table: "Post",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Post",
                keyColumn: "Discuss",
                keyValue: null,
                column: "Discuss",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Discuss",
                table: "Post",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_PostType_PostTypeId",
                table: "Post",
                column: "PostTypeId",
                principalTable: "PostType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_PostType_PostTypeId",
                table: "Post");

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "Post",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PostTypeId",
                table: "Post",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Discuss",
                table: "Post",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_PostType_PostTypeId",
                table: "Post",
                column: "PostTypeId",
                principalTable: "PostType",
                principalColumn: "Id");
        }
    }
}
