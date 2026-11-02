using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmacy.System.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameInventoryItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItem_Products_ProductId",
                table: "InventoryItem");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItem_Repositories_RepositoryId",
                table: "InventoryItem");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItem_SupplyItem_SupplyItemId",
                table: "InventoryItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryItem",
                table: "InventoryItem");

            migrationBuilder.RenameTable(
                name: "InventoryItem",
                newName: "InventoryItems");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryItem_SupplyItemId",
                table: "InventoryItems",
                newName: "IX_InventoryItems_SupplyItemId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryItem_RepositoryId",
                table: "InventoryItems",
                newName: "IX_InventoryItems_RepositoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryItem_ProductId",
                table: "InventoryItems",
                newName: "IX_InventoryItems_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryItems",
                table: "InventoryItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Products_ProductId",
                table: "InventoryItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Repositories_RepositoryId",
                table: "InventoryItems",
                column: "RepositoryId",
                principalTable: "Repositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_SupplyItem_SupplyItemId",
                table: "InventoryItems",
                column: "SupplyItemId",
                principalTable: "SupplyItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Products_ProductId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Repositories_RepositoryId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_SupplyItem_SupplyItemId",
                table: "InventoryItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryItems",
                table: "InventoryItems");

            migrationBuilder.RenameTable(
                name: "InventoryItems",
                newName: "InventoryItem");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryItems_SupplyItemId",
                table: "InventoryItem",
                newName: "IX_InventoryItem_SupplyItemId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryItems_RepositoryId",
                table: "InventoryItem",
                newName: "IX_InventoryItem_RepositoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryItems_ProductId",
                table: "InventoryItem",
                newName: "IX_InventoryItem_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryItem",
                table: "InventoryItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItem_Products_ProductId",
                table: "InventoryItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItem_Repositories_RepositoryId",
                table: "InventoryItem",
                column: "RepositoryId",
                principalTable: "Repositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItem_SupplyItem_SupplyItemId",
                table: "InventoryItem",
                column: "SupplyItemId",
                principalTable: "SupplyItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
