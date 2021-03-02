using Microsoft.EntityFrameworkCore.Migrations;

namespace ProxyAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proxies",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EndPoint = table.Column<string>(type: "TEXT", nullable: true),
                    Region = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proxies", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Proxies");
        }
    }
}
