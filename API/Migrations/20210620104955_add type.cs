using Microsoft.EntityFrameworkCore.Migrations;

namespace ProxyAPI.Migrations
{
    public partial class addtype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WiFiAccessPoints_WiFiClients_WiFiClientId",
                table: "WiFiAccessPoints");

            migrationBuilder.DropIndex(
                name: "IX_WiFiAccessPoints_WiFiClientId",
                table: "WiFiAccessPoints");

            migrationBuilder.DropColumn(
                name: "WiFiClientId",
                table: "WiFiAccessPoints");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Proxies",
                newName: "Id");

            migrationBuilder.AddColumn<ulong>(
                name: "ClientWiFiClientId",
                table: "WiFiProbes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Proxies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WiFiAccessPointWiFiClient",
                columns: table => new
                {
                    AccessPointsWiFiAccessPointId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ClientsWiFiClientId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiFiAccessPointWiFiClient", x => new { x.AccessPointsWiFiAccessPointId, x.ClientsWiFiClientId });
                    table.ForeignKey(
                        name: "FK_WiFiAccessPointWiFiClient_WiFiAccessPoints_AccessPointsWiFiAccessPointId",
                        column: x => x.AccessPointsWiFiAccessPointId,
                        principalTable: "WiFiAccessPoints",
                        principalColumn: "WiFiAccessPointId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WiFiAccessPointWiFiClient_WiFiClients_ClientsWiFiClientId",
                        column: x => x.ClientsWiFiClientId,
                        principalTable: "WiFiClients",
                        principalColumn: "WiFiClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WiFiProbes_ClientWiFiClientId",
                table: "WiFiProbes",
                column: "ClientWiFiClientId");

            migrationBuilder.CreateIndex(
                name: "IX_WiFiAccessPointWiFiClient_ClientsWiFiClientId",
                table: "WiFiAccessPointWiFiClient",
                column: "ClientsWiFiClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_WiFiProbes_WiFiClients_ClientWiFiClientId",
                table: "WiFiProbes",
                column: "ClientWiFiClientId",
                principalTable: "WiFiClients",
                principalColumn: "WiFiClientId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WiFiProbes_WiFiClients_ClientWiFiClientId",
                table: "WiFiProbes");

            migrationBuilder.DropTable(
                name: "WiFiAccessPointWiFiClient");

            migrationBuilder.DropIndex(
                name: "IX_WiFiProbes_ClientWiFiClientId",
                table: "WiFiProbes");

            migrationBuilder.DropColumn(
                name: "ClientWiFiClientId",
                table: "WiFiProbes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Proxies");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Proxies",
                newName: "ID");

            migrationBuilder.AddColumn<ulong>(
                name: "WiFiClientId",
                table: "WiFiAccessPoints",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WiFiAccessPoints_WiFiClientId",
                table: "WiFiAccessPoints",
                column: "WiFiClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_WiFiAccessPoints_WiFiClients_WiFiClientId",
                table: "WiFiAccessPoints",
                column: "WiFiClientId",
                principalTable: "WiFiClients",
                principalColumn: "WiFiClientId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
