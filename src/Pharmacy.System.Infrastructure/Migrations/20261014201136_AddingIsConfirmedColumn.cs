using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmacy.System.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingIsConfirmedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "Supplies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Supplies");
        }
    }
}
