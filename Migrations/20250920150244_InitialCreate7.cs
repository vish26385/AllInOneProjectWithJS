using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllInOneProject.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseMaster_PartyMasters_PartyId",
                table: "PurchaseMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesDet_Items_itemId",
                table: "SalesDet");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesMas_PartyMasters_PartyId",
                table: "SalesMas");

            migrationBuilder.RenameColumn(
                name: "PartyId",
                table: "SalesMas",
                newName: "PartyMasterId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesMas_PartyId",
                table: "SalesMas",
                newName: "IX_SalesMas_PartyMasterId");

            migrationBuilder.RenameColumn(
                name: "itemId",
                table: "SalesDet",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesDet_itemId",
                table: "SalesDet",
                newName: "IX_SalesDet_ItemId");

            migrationBuilder.RenameColumn(
                name: "PartyId",
                table: "PurchaseMaster",
                newName: "PartyMasterId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseMaster_PartyId",
                table: "PurchaseMaster",
                newName: "IX_PurchaseMaster_PartyMasterId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Carts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_AspNetUsers_UserId",
                table: "Carts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseMaster_PartyMasters_PartyMasterId",
                table: "PurchaseMaster",
                column: "PartyMasterId",
                principalTable: "PartyMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesDet_Items_ItemId",
                table: "SalesDet",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesMas_PartyMasters_PartyMasterId",
                table: "SalesMas",
                column: "PartyMasterId",
                principalTable: "PartyMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_AspNetUsers_UserId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseMaster_PartyMasters_PartyMasterId",
                table: "PurchaseMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesDet_Items_ItemId",
                table: "SalesDet");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesMas_PartyMasters_PartyMasterId",
                table: "SalesMas");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Carts_UserId",
                table: "Carts");

            migrationBuilder.RenameColumn(
                name: "PartyMasterId",
                table: "SalesMas",
                newName: "PartyId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesMas_PartyMasterId",
                table: "SalesMas",
                newName: "IX_SalesMas_PartyId");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "SalesDet",
                newName: "itemId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesDet_ItemId",
                table: "SalesDet",
                newName: "IX_SalesDet_itemId");

            migrationBuilder.RenameColumn(
                name: "PartyMasterId",
                table: "PurchaseMaster",
                newName: "PartyId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseMaster_PartyMasterId",
                table: "PurchaseMaster",
                newName: "IX_PurchaseMaster_PartyId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Carts",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseMaster_PartyMasters_PartyId",
                table: "PurchaseMaster",
                column: "PartyId",
                principalTable: "PartyMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesDet_Items_itemId",
                table: "SalesDet",
                column: "itemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesMas_PartyMasters_PartyId",
                table: "SalesMas",
                column: "PartyId",
                principalTable: "PartyMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
