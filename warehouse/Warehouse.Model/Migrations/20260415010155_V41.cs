using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V41 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentPatternId",
                schema: "wh",
                table: "StockFlow",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasContractNo",
                schema: "wh",
                table: "DocumentPatterns",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_DocumentPatternId",
                schema: "wh",
                table: "StockFlow",
                column: "DocumentPatternId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockFlow_DocumentPatterns_DocumentPatternId",
                schema: "wh",
                table: "StockFlow",
                column: "DocumentPatternId",
                principalSchema: "wh",
                principalTable: "DocumentPatterns",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockFlow_DocumentPatterns_DocumentPatternId",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropIndex(
                name: "IX_StockFlow_DocumentPatternId",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropColumn(
                name: "DocumentPatternId",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropColumn(
                name: "HasContractNo",
                schema: "wh",
                table: "DocumentPatterns");
        }
    }
}
