using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class modify4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChapterEndPosition",
                table: "chapter",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ChapterStartPosition",
                table: "chapter",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChapterEndPosition",
                table: "chapter");

            migrationBuilder.DropColumn(
                name: "ChapterStartPosition",
                table: "chapter");
        }
    }
}
