using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class modify3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "novel",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValue: "0");

            migrationBuilder.AlterColumn<int>(
                name: "FromType",
                table: "novel",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValue: "0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "novel",
                nullable: true,
                defaultValue: "0",
                oldClrType: typeof(int),
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "FromType",
                table: "novel",
                nullable: true,
                defaultValue: "0",
                oldClrType: typeof(int),
                oldDefaultValue: 0);
        }
    }
}
