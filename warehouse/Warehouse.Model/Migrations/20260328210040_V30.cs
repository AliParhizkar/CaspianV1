using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V30 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "wh",
                table: "PurchaseRequests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "GoodsAccesses",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsAccessType = table.Column<byte>(type: "tinyint", nullable: false),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    StockRoomId = table.Column<int>(type: "int", nullable: true),
                    GoodsPlacementId = table.Column<int>(type: "int", nullable: true)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodsAccesses",
                schema: "wh");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "wh",
                table: "PurchaseRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
