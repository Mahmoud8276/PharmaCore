using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmacy.System.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRepositoryAndPaymentRelatedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserPaidConfirmation",
                table: "Supplies");

            migrationBuilder.AddColumn<int>(
                name: "PaymentPlan",
                table: "Supplies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RepositoryId",
                table: "Supplies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Repositories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMain = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repositories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupplyInstallments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplyId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyInstallments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplyInstallments_Supplies_SupplyId",
                        column: x => x.SupplyId,
                        principalTable: "Supplies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplyPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplyInstallmentId = table.Column<int>(type: "int", nullable: true),
                    SupplyId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplyPayments_Supplies_SupplyId",
                        column: x => x.SupplyId,
                        principalTable: "Supplies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplyPayments_SupplyInstallments_SupplyInstallmentId",
                        column: x => x.SupplyInstallmentId,
                        principalTable: "SupplyInstallments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Supplies_RepositoryId",
                table: "Supplies",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyInstallments_SupplyId",
                table: "SupplyInstallments",
                column: "SupplyId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyPayments_SupplyId",
                table: "SupplyPayments",
                column: "SupplyId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyPayments_SupplyInstallmentId",
                table: "SupplyPayments",
                column: "SupplyInstallmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Supplies_Repositories_RepositoryId",
                table: "Supplies",
                column: "RepositoryId",
                principalTable: "Repositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Supplies_Repositories_RepositoryId",
                table: "Supplies");

            migrationBuilder.DropTable(
                name: "Repositories");

            migrationBuilder.DropTable(
                name: "SupplyPayments");

            migrationBuilder.DropTable(
                name: "SupplyInstallments");

            migrationBuilder.DropIndex(
                name: "IX_Supplies_RepositoryId",
                table: "Supplies");

            migrationBuilder.DropColumn(
                name: "PaymentPlan",
                table: "Supplies");

            migrationBuilder.DropColumn(
                name: "RepositoryId",
                table: "Supplies");

            migrationBuilder.AddColumn<bool>(
                name: "UserPaidConfirmation",
                table: "Supplies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
