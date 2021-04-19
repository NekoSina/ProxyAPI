using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProxyAPI.Migrations
{
    public partial class recreatedb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proxies",
                columns: table => new
                {
                    ID = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IP = table.Column<string>(type: "TEXT", nullable: true),
                    Port = table.Column<ushort>(type: "INTEGER", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true),
                    Region = table.Column<string>(type: "TEXT", nullable: true),
                    Longitude = table.Column<float>(type: "REAL", nullable: false),
                    Latitude = table.Column<float>(type: "REAL", nullable: false),
                    LastTest = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AS = table.Column<string>(type: "TEXT", nullable: true),
                    ASN = table.Column<string>(type: "TEXT", nullable: true),
                    Domain = table.Column<string>(type: "TEXT", nullable: true),
                    ISP = table.Column<string>(type: "TEXT", nullable: true),
                    ProxyType = table.Column<string>(type: "TEXT", nullable: true),
                    Threat = table.Column<string>(type: "TEXT", nullable: true),
                    Score = table.Column<int>(type: "INTEGER", nullable: false),
                    Working = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proxies", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "WiFiMacs",
                columns: table => new
                {
                    WiFiMacId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MAC = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiFiMacs", x => x.WiFiMacId);
                });

            migrationBuilder.CreateTable(
                name: "WiFiNetworkNames",
                columns: table => new
                {
                    WiFiNetworkNameId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SSID = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiFiNetworkNames", x => x.WiFiNetworkNameId);
                });

            migrationBuilder.CreateTable(
                name: "WiFiClients",
                columns: table => new
                {
                    WiFiClientId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WiFiMacId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    LastSeen = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiFiClients", x => x.WiFiClientId);
                    table.ForeignKey(
                        name: "FK_WiFiClients_WiFiMacs_WiFiMacId",
                        column: x => x.WiFiMacId,
                        principalTable: "WiFiMacs",
                        principalColumn: "WiFiMacId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WiFiProbes",
                columns: table => new
                {
                    WiFiProbeId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WiFiMacId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    WiFiNetworkNameId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    LastSeen = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiFiProbes", x => x.WiFiProbeId);
                    table.ForeignKey(
                        name: "FK_WiFiProbes_WiFiMacs_WiFiMacId",
                        column: x => x.WiFiMacId,
                        principalTable: "WiFiMacs",
                        principalColumn: "WiFiMacId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WiFiProbes_WiFiNetworkNames_WiFiNetworkNameId",
                        column: x => x.WiFiNetworkNameId,
                        principalTable: "WiFiNetworkNames",
                        principalColumn: "WiFiNetworkNameId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WiFiAccessPoints",
                columns: table => new
                {
                    WiFiAccessPointId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WiFiMacId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    WiFiNetworkNameId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    LastSeen = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WiFiClientId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiFiAccessPoints", x => x.WiFiAccessPointId);
                    table.ForeignKey(
                        name: "FK_WiFiAccessPoints_WiFiClients_WiFiClientId",
                        column: x => x.WiFiClientId,
                        principalTable: "WiFiClients",
                        principalColumn: "WiFiClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WiFiAccessPoints_WiFiMacs_WiFiMacId",
                        column: x => x.WiFiMacId,
                        principalTable: "WiFiMacs",
                        principalColumn: "WiFiMacId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WiFiAccessPoints_WiFiNetworkNames_WiFiNetworkNameId",
                        column: x => x.WiFiNetworkNameId,
                        principalTable: "WiFiNetworkNames",
                        principalColumn: "WiFiNetworkNameId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WiFiAccessPoints_WiFiClientId",
                table: "WiFiAccessPoints",
                column: "WiFiClientId");

            migrationBuilder.CreateIndex(
                name: "IX_WiFiAccessPoints_WiFiMacId",
                table: "WiFiAccessPoints",
                column: "WiFiMacId");

            migrationBuilder.CreateIndex(
                name: "IX_WiFiAccessPoints_WiFiNetworkNameId",
                table: "WiFiAccessPoints",
                column: "WiFiNetworkNameId");

            migrationBuilder.CreateIndex(
                name: "IX_WiFiClients_WiFiMacId",
                table: "WiFiClients",
                column: "WiFiMacId");

            migrationBuilder.CreateIndex(
                name: "IX_WiFiProbes_WiFiMacId",
                table: "WiFiProbes",
                column: "WiFiMacId");

            migrationBuilder.CreateIndex(
                name: "IX_WiFiProbes_WiFiNetworkNameId",
                table: "WiFiProbes",
                column: "WiFiNetworkNameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Proxies");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WiFiAccessPoints");

            migrationBuilder.DropTable(
                name: "WiFiProbes");

            migrationBuilder.DropTable(
                name: "WiFiClients");

            migrationBuilder.DropTable(
                name: "WiFiNetworkNames");

            migrationBuilder.DropTable(
                name: "WiFiMacs");
        }
    }
}
