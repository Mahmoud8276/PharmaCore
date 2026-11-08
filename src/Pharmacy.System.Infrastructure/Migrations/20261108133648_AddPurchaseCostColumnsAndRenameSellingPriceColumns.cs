using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmacy.System.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseCostColumnsAndRenameSellingPriceColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PricePerUnit",
                table: "SupplyItem",
                newName: "SellingPricePerReceivedUnit");

            migrationBuilder.RenameColumn(
                name: "PricePerBaseUnits",
                table: "InventoryItems",
                newName: "SellingPricePerBaseUnit");

            migrationBuilder.AddColumn<decimal>(
                name: "PurchaseCostPerReceivedUnit",
                table: "SupplyItem",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchaseCostPerBaseUnit",
                table: "InventoryItems",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseCostPerReceivedUnit",
                table: "SupplyItem");

            migrationBuilder.DropColumn(
                name: "PurchaseCostPerBaseUnit",
                table: "InventoryItems");

            migrationBuilder.RenameColumn(
                name: "SellingPricePerReceivedUnit",
                table: "SupplyItem",
                newName: "PricePerUnit");

            migrationBuilder.RenameColumn(
                name: "SellingPricePerBaseUnit",
                table: "InventoryItems",
                newName: "PricePerBaseUnits");
        }
    }
}
