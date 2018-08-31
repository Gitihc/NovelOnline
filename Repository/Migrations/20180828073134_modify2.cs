using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class modify2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_web_sitenovel",
                table: "web_sitenovel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_web_site",
                table: "web_site");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_novel",
                table: "user_novel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_module_element",
                table: "module_element");

            migrationBuilder.RenameTable(
                name: "web_sitenovel",
                newName: "websitenovel");

            migrationBuilder.RenameTable(
                name: "web_site",
                newName: "website");

            migrationBuilder.RenameTable(
                name: "user_novel",
                newName: "usernovel");

            migrationBuilder.RenameTable(
                name: "module_element",
                newName: "moduleelement");

            migrationBuilder.AddPrimaryKey(
                name: "PK_websitenovel",
                table: "websitenovel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_website",
                table: "website",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_usernovel",
                table: "usernovel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_moduleelement",
                table: "moduleelement",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_websitenovel",
                table: "websitenovel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_website",
                table: "website");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usernovel",
                table: "usernovel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_moduleelement",
                table: "moduleelement");

            migrationBuilder.RenameTable(
                name: "websitenovel",
                newName: "web_sitenovel");

            migrationBuilder.RenameTable(
                name: "website",
                newName: "web_site");

            migrationBuilder.RenameTable(
                name: "usernovel",
                newName: "user_novel");

            migrationBuilder.RenameTable(
                name: "moduleelement",
                newName: "module_element");

            migrationBuilder.AddPrimaryKey(
                name: "PK_web_sitenovel",
                table: "web_sitenovel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_web_site",
                table: "web_site",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_novel",
                table: "user_novel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_module_element",
                table: "module_element",
                column: "Id");
        }
    }
}
