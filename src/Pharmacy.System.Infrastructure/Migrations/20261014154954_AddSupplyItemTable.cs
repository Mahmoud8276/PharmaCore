using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmacy.System.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplyItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountPercintage",
                table: "Supplies",
                newName: "FinalCost");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCost",
                table: "Supplies",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ReceivedAt",
                table: "Supplies",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "Supplies",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "UserPaidConfirmation",
                table: "Supplies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SupplyItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplyId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    BaseUnitsPerReceivedUnit = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricePerUnit = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EffectiveUnitCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    PackagingLevelId = table.Column<int>(type: "int", nullable: true),
                    PackagingLevelsSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplyItem_ProductPackagingLevels_PackagingLevelId",
                        column: x => x.PackagingLevelId,
                        principalTable: "ProductPackagingLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyItem_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyItem_Supplies_SupplyId",
                        column: x => x.SupplyId,
                        principalTable: "Supplies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyItem_PackagingLevelId",
                table: "SupplyItem",
                column: "PackagingLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyItem_ProductId",
                table: "SupplyItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyItem_SupplyId",
                table: "SupplyItem",
                column: "SupplyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplyItem");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Supplies");

            migrationBuilder.DropColumn(
                name: "UserPaidConfirmation",
                table: "Supplies");

            migrationBuilder.RenameColumn(
                name: "FinalCost",
                table: "Supplies",
                newName: "DiscountPercintage");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCost",
                table: "Supplies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReceivedAt",
                table: "Supplies",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
