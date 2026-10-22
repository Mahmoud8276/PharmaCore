using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmacy.System.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultPackagingToProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultPackagingId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_DefaultPackagingId",
                table: "Products",
                column: "DefaultPackagingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductPackagingLevels_DefaultPackagingId",
                table: "Products",
                column: "DefaultPackagingId",
                principalTable: "ProductPackagingLevels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductPackagingLevels_DefaultPackagingId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_DefaultPackagingId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DefaultPackagingId",
                table: "Products");
        }
    }
}
