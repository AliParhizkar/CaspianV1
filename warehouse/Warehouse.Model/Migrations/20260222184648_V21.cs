using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubstituteProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    SubstituteGoodsId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(6,3)", precision: 6, scale: 3, nullable: false),
                    DuplicateRelation = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubstituteProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubstituteProduct_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubstituteProduct_Goods_SubstituteGoodsId",
                        column: x => x.SubstituteGoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubstituteProduct_GoodsId",
                table: "SubstituteProduct",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_SubstituteProduct_SubstituteGoodsId",
                table: "SubstituteProduct",
                column: "SubstituteGoodsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubstituteProduct");
        }
    }
}
