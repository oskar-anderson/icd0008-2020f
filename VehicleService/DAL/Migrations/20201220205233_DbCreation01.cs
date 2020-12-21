using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class DbCreation01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Price = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StockParts",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Category = table.Column<string>(maxLength: 256, nullable: false),
                    CurrentQuantity = table.Column<int>(nullable: false),
                    OptimalQuantity = table.Column<int>(nullable: false),
                    Location = table.Column<string>(maxLength: 256, nullable: false),
                    Price = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockParts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTickets",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    ServiceID = table.Column<string>(maxLength: 36, nullable: false),
                    Vehicle = table.Column<string>(maxLength: 256, nullable: false),
                    RequestedBy = table.Column<string>(maxLength: 256, nullable: false),
                    IsDone = table.Column<bool>(nullable: false),
                    MechanicsNames = table.Column<string>(maxLength: 256, nullable: false),
                    Comment = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTickets", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceTickets_Services_ServiceID",
                        column: x => x.ServiceID,
                        principalTable: "Services",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ServiceStockParts",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    StockPartID = table.Column<string>(maxLength: 36, nullable: false),
                    ServiceID = table.Column<string>(maxLength: 36, nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceStockParts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceStockParts_Services_ServiceID",
                        column: x => x.ServiceID,
                        principalTable: "Services",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ServiceStockParts_StockParts_StockPartID",
                        column: x => x.StockPartID,
                        principalTable: "StockParts",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceStockParts_ServiceID",
                table: "ServiceStockParts",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceStockParts_StockPartID",
                table: "ServiceStockParts",
                column: "StockPartID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTickets_ServiceID",
                table: "ServiceTickets",
                column: "ServiceID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceStockParts");

            migrationBuilder.DropTable(
                name: "ServiceTickets");

            migrationBuilder.DropTable(
                name: "StockParts");

            migrationBuilder.DropTable(
                name: "Services");
        }
    }
}
