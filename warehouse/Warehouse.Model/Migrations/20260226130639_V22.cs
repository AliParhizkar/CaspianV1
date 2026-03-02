using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubstituteProduct_Goods_GoodsId",
                table: "SubstituteProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_SubstituteProduct_Goods_SubstituteGoodsId",
                table: "SubstituteProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubstituteProduct",
                table: "SubstituteProduct");

            migrationBuilder.RenameTable(
                name: "SubstituteProduct",
                newName: "SubstituteProducts",
                newSchema: "wh");

            migrationBuilder.RenameIndex(
                name: "IX_SubstituteProduct_SubstituteGoodsId",
                schema: "wh",
                table: "SubstituteProducts",
                newName: "IX_SubstituteProducts_SubstituteGoodsId");

            migrationBuilder.RenameIndex(
                name: "IX_SubstituteProduct_GoodsId",
                schema: "wh",
                table: "SubstituteProducts",
                newName: "IX_SubstituteProducts_GoodsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubstituteProducts",
                schema: "wh",
                table: "SubstituteProducts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubstituteProducts_Goods_GoodsId",
                schema: "wh",
                table: "SubstituteProducts",
                column: "GoodsId",
                principalSchema: "wh",
                principalTable: "Goods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubstituteProducts_Goods_SubstituteGoodsId",
                schema: "wh",
                table: "SubstituteProducts",
                column: "SubstituteGoodsId",
                principalSchema: "wh",
                principalTable: "Goods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubstituteProducts_Goods_GoodsId",
                schema: "wh",
                table: "SubstituteProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_SubstituteProducts_Goods_SubstituteGoodsId",
                schema: "wh",
                table: "SubstituteProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubstituteProducts",
                schema: "wh",
                table: "SubstituteProducts");

            migrationBuilder.RenameTable(
                name: "SubstituteProducts",
                schema: "wh",
                newName: "SubstituteProduct");

            migrationBuilder.RenameIndex(
                name: "IX_SubstituteProducts_SubstituteGoodsId",
                table: "SubstituteProduct",
                newName: "IX_SubstituteProduct_SubstituteGoodsId");

            migrationBuilder.RenameIndex(
                name: "IX_SubstituteProducts_GoodsId",
                table: "SubstituteProduct",
                newName: "IX_SubstituteProduct_GoodsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubstituteProduct",
                table: "SubstituteProduct",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubstituteProduct_Goods_GoodsId",
                table: "SubstituteProduct",
                column: "GoodsId",
                principalSchema: "wh",
                principalTable: "Goods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubstituteProduct_Goods_SubstituteGoodsId",
                table: "SubstituteProduct",
                column: "SubstituteGoodsId",
                principalSchema: "wh",
                principalTable: "Goods",
                principalColumn: "Id");
        }
    }
}
