using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "wh",
                table: "Goods",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_SupplierId",
                schema: "wh",
                table: "StockFlow",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_SupplierId",
                schema: "wh",
                table: "Reservations",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Suppliers_SupplierId",
                schema: "wh",
                table: "Reservations",
                column: "SupplierId",
                principalSchema: "pcm",
                principalTable: "Suppliers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockFlow_Suppliers_SupplierId",
                schema: "wh",
                table: "StockFlow",
                column: "SupplierId",
                principalSchema: "pcm",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Suppliers_SupplierId",
                schema: "wh",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_StockFlow_Suppliers_SupplierId",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropIndex(
                name: "IX_StockFlow_SupplierId",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_SupplierId",
                schema: "wh",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "wh",
                table: "Goods");
        }
    }
}
