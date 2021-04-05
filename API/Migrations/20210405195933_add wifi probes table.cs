using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HerstAPI.Migrations
{
    public partial class addwifiprobestable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WiFiProbes",
                columns: table => new
                {
                    MAC = table.Column<string>(type: "TEXT", nullable: false),
                    SSID = table.Column<string>(type: "TEXT", nullable: true),
                    LastSeen = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiFiProbes", x => x.MAC);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WiFiProbes");
        }
    }
}
