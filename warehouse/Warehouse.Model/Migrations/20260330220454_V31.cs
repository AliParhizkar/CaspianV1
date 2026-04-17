using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V31 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodsAccesses",
                schema: "wh");

            migrationBuilder.CreateTable(
                name: "PurchaseRequestsGoods",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseRequestId = table.Column<int>(type: "int", nullable: false),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float(15)", precision: 15, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequestsGoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestsGoods_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequestsGoods_PurchaseRequests_PurchaseRequestId",
                        column: x => x.PurchaseRequestId,
                        principalSchema: "wh",
                        principalTable: "PurchaseRequests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReservationGoods",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float(15)", precision: 15, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationGoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationGoods_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReservationGoods_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalSchema: "wh",
                        principalTable: "Reservations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_GoodsId",
                schema: "wh",
                table: "PurchaseRequestsGoods",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_PurchaseRequestId",
                schema: "wh",
                table: "PurchaseRequestsGoods",
                column: "PurchaseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationGoods_GoodsId",
                schema: "wh",
                table: "ReservationGoods",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationGoods_ReservationId",
                schema: "wh",
                table: "ReservationGoods",
                column: "ReservationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseRequestsGoods",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "ReservationGoods",
                schema: "wh");

            migrationBuilder.CreateTable(
                name: "GoodsAccesses",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    GoodsPlacementId = table.Column<int>(type: "int", nullable: true),
                    StockRoomId = table.Column<int>(type: "int", nullable: true),
                    GoodsAccessType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsAccesses_GoodsPlacements_GoodsPlacementId",
                        column: x => x.GoodsPlacementId,
                        principalSchema: "wh",
                        principalTable: "GoodsPlacements",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodsAccesses_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodsAccesses_StockRooms_StockRoomId",
                        column: x => x.StockRoomId,
                        principalSchema: "wh",
                        principalTable: "StockRooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsAccesses_GoodsId",
                schema: "wh",
                table: "GoodsAccesses",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsAccesses_GoodsPlacementId",
                schema: "wh",
                table: "GoodsAccesses",
                column: "GoodsPlacementId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsAccesses_StockRoomId",
                schema: "wh",
                table: "GoodsAccesses",
                column: "StockRoomId");
        }
    }
}
