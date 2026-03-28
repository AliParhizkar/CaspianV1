using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GoodsPlacements",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockRoomId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsPlacements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsPlacements_MaterialLocations_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "wh",
                        principalTable: "MaterialLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodsPlacements_StockRooms_StockRoomId",
                        column: x => x.StockRoomId,
                        principalSchema: "wh",
                        principalTable: "StockRooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsPlacements_LocationId",
                schema: "wh",
                table: "GoodsPlacements",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsPlacements_StockRoomId",
                schema: "wh",
                table: "GoodsPlacements",
                column: "StockRoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodsPlacements",
                schema: "wh");
        }
    }
}
