using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PropertiesOfGoods",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsPropertiesId = table.Column<int>(type: "int", nullable: false),
                    GoodsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertiesOfGoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertiesOfGoods_GoodsProperties_GoodsPropertiesId",
                        column: x => x.GoodsPropertiesId,
                        principalSchema: "wh",
                        principalTable: "GoodsProperties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropertiesOfGoods_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertiesOfGoods_GoodsId",
                schema: "wh",
                table: "PropertiesOfGoods",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertiesOfGoods_GoodsPropertiesId",
                schema: "wh",
                table: "PropertiesOfGoods",
                column: "GoodsPropertiesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertiesOfGoods",
                schema: "wh");
        }
    }
}
