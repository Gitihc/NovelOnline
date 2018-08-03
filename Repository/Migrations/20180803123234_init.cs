using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chapter",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    NovelId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    SourceUrl = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Sort = table.Column<int>(nullable: false, defaultValue: 1),
                    State = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chapter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "menu",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    ParentId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Type = table.Column<int>(nullable: false, defaultValue: 0),
                    Link = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    IsEnable = table.Column<int>(nullable: false, defaultValue: 1),
                    Sort = table.Column<int>(nullable: false, defaultValue: 1),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatorId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "novel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    SourceUrl = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    State = table.Column<string>(nullable: true, defaultValue: "0"),
                    Type = table.Column<int>(nullable: false, defaultValue: 0),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_novel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    Account = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    Password = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    Name = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    Sex = table.Column<int>(nullable: false, defaultValue: 0),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatorId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_novel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    NovelId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
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
                    Url = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
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
                    NovelUrl = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
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
                name: "menu");

            migrationBuilder.DropTable(
                name: "novel");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "user_novel");

            migrationBuilder.DropTable(
                name: "web_site");

            migrationBuilder.DropTable(
                name: "web_sitenovel");
        }
    }
}
