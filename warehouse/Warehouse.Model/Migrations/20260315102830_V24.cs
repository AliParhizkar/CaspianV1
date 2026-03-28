using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GoodsId",
                schema: "wh",
                table: "GoodsPlacements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsPlacements_GoodsId",
                schema: "wh",
                table: "GoodsPlacements",
                column: "GoodsId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsPlacements_Goods_GoodsId",
                schema: "wh",
                table: "GoodsPlacements",
                column: "GoodsId",
                principalSchema: "wh",
                principalTable: "Goods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsPlacements_Goods_GoodsId",
                schema: "wh",
                table: "GoodsPlacements");

            migrationBuilder.DropIndex(
                name: "IX_GoodsPlacements_GoodsId",
                schema: "wh",
                table: "GoodsPlacements");

            migrationBuilder.DropColumn(
                name: "GoodsId",
                schema: "wh",
                table: "GoodsPlacements");
        }
    }
}
