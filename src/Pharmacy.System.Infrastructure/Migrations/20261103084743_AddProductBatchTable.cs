using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmacy.System.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductBatchTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductBatchId",
                table: "SupplyItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductBatchId",
                table: "InventoryItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProductBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ManufactureDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBatches", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyItem_ProductBatchId",
                table: "SupplyItem",
                column: "ProductBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_ProductBatchId",
                table: "InventoryItems",
                column: "ProductBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_Number",
                table: "ProductBatches",
                column: "Number",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_ProductBatches_ProductBatchId",
                table: "InventoryItems",
                column: "ProductBatchId",
                principalTable: "ProductBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplyItem_ProductBatches_ProductBatchId",
                table: "SupplyItem",
                column: "ProductBatchId",
                principalTable: "ProductBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_ProductBatches_ProductBatchId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplyItem_ProductBatches_ProductBatchId",
                table: "SupplyItem");

            migrationBuilder.DropTable(
                name: "ProductBatches");

            migrationBuilder.DropIndex(
                name: "IX_SupplyItem_ProductBatchId",
                table: "SupplyItem");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_ProductBatchId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "ProductBatchId",
                table: "SupplyItem");

            migrationBuilder.DropColumn(
                name: "ProductBatchId",
                table: "InventoryItems");
        }
    }
}
