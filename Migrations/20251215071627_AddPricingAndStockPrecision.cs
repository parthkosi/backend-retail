using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingAndStockPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostPrice",
                table: "PurchaseRequestItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Mrp",
                table: "PurchaseRequestItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "PurchaseRequestItems");

            migrationBuilder.DropColumn(
                name: "Mrp",
                table: "PurchaseRequestItems");
        }
    }
}
