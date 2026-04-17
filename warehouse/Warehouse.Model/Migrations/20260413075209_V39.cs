using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V39 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountingCodeId",
                schema: "wh",
                table: "StockFlow",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "StockFlowType",
                schema: "wh",
                table: "StockFlow",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "GoodsFlow",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockFlowId = table.Column<int>(type: "int", nullable: false),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsFlow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsFlow_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodsFlow_StockFlow_StockFlowId",
                        column: x => x.StockFlowId,
                        principalSchema: "wh",
                        principalTable: "StockFlow",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_AccountingCodeId",
                schema: "wh",
                table: "StockFlow",
                column: "AccountingCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsFlow_GoodsId",
                schema: "wh",
                table: "GoodsFlow",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsFlow_StockFlowId",
                schema: "wh",
                table: "GoodsFlow",
                column: "StockFlowId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockFlow_AccountingCodes_AccountingCodeId",
                schema: "wh",
                table: "StockFlow",
                column: "AccountingCodeId",
                principalSchema: "acc",
                principalTable: "AccountingCodes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockFlow_AccountingCodes_AccountingCodeId",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropTable(
                name: "GoodsFlow",
                schema: "wh");

            migrationBuilder.DropIndex(
                name: "IX_StockFlow_AccountingCodeId",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropColumn(
                name: "AccountingCodeId",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropColumn(
                name: "StockFlowType",
                schema: "wh",
                table: "StockFlow");
        }
    }
}
