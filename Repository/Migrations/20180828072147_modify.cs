using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class modify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chapter",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    NovelId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    NovelName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    OriginLink = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Sort = table.Column<int>(nullable: false, defaultValue: 1),
                    State = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chapter", x => x.Id);
                });

         

            

            migrationBuilder.CreateTable(
                name: "novel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    PhysicalPath = table.Column<string>(nullable: true),
                    OriginLink = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    FromType = table.Column<string>(nullable: true, defaultValue: "0"),
                    State = table.Column<string>(nullable: true, defaultValue: "0"),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_novel", x => x.Id);
                });

       

          

            migrationBuilder.CreateTable(
                name: "user_novel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    NovelId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    NovelName = table.Column<string>(nullable: true),
                    LastOpenTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    State = table.Column<int>(nullable: false, defaultValue: 0),
                    LastChapterId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_novel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "web_site",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    OriginLink = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    State = table.Column<int>(nullable: false, defaultValue: 0),
                    CreatorId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_web_site", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "web_sitenovel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    WebSiteId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Author = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    OriginLink = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    State = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_web_sitenovel", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chapter");

          
            migrationBuilder.DropTable(
                name: "novel");

         

       
            migrationBuilder.DropTable(
                name: "user_novel");

            migrationBuilder.DropTable(
                name: "web_site");

            migrationBuilder.DropTable(
                name: "web_sitenovel");
        }
    }
}
