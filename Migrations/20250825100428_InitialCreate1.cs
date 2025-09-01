using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllInOneProject.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartyMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesMas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDays = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PartyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesMas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesMas_PartyMasters_PartyId",
                        column: x => x.PartyId,
                        principalTable: "PartyMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesDet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesMasterId = table.Column<int>(type: "int", nullable: false),
                    itemId = table.Column<int>(type: "int", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesDet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesDet_Items_itemId",
                        column: x => x.itemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesDet_SalesMas_SalesMasterId",
                        column: x => x.SalesMasterId,
                        principalTable: "SalesMas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carts_ItemId",
                table: "Carts",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesDet_itemId",
                table: "SalesDet",
                column: "itemId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesDet_SalesMasterId",
                table: "SalesDet",
                column: "SalesMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesMas_PartyId",
                table: "SalesMas",
                column: "PartyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Items_ItemId",
                table: "Carts",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Items_ItemId",
                table: "Carts");

            migrationBuilder.DropTable(
                name: "SalesDet");

            migrationBuilder.DropTable(
                name: "SalesMas");

            migrationBuilder.DropTable(
                name: "PartyMasters");

            migrationBuilder.DropIndex(
                name: "IX_Carts_ItemId",
                table: "Carts");
        }
    }
}
