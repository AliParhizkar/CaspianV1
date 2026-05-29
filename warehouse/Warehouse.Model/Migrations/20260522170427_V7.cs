using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GoodsOrderings",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    KeepCenterId = table.Column<int>(type: "int", nullable: false),
                    MinimumInventory = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    MaximumInventory = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    OrderingPoint = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    EconomicOrder = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ConsumeUltimate = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsOrderings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsOrderings_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodsOrderings_KeepCenters_KeepCenterId",
                        column: x => x.KeepCenterId,
                        principalSchema: "wh",
                        principalTable: "KeepCenters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsOrderings_GoodsId",
                schema: "wh",
                table: "GoodsOrderings",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsOrderings_KeepCenterId",
                schema: "wh",
                table: "GoodsOrderings",
                column: "KeepCenterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodsOrderings",
                schema: "wh");
        }
    }
}
