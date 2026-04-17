using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V35 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CostCenterType",
                schema: "wh",
                table: "PurchaseRequests",
                newName: "OtherPartyType");

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                schema: "wh",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                schema: "wh",
                table: "PurchaseRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StockFlow",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeepCenterId = table.Column<int>(type: "int", nullable: false),
                    StockRoomId = table.Column<int>(type: "int", nullable: true),
                    OtherKeepCenterId = table.Column<int>(type: "int", nullable: true),
                    OtherStockRoomId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockFlow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockFlow_KeepCenters_KeepCenterId",
                        column: x => x.KeepCenterId,
                        principalSchema: "wh",
                        principalTable: "KeepCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockFlow_KeepCenters_OtherKeepCenterId",
                        column: x => x.OtherKeepCenterId,
                        principalSchema: "wh",
                        principalTable: "KeepCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockFlow_StockRooms_OtherStockRoomId",
                        column: x => x.OtherStockRoomId,
                        principalSchema: "wh",
                        principalTable: "StockRooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockFlow_StockRooms_StockRoomId",
                        column: x => x.StockRoomId,
                        principalSchema: "wh",
                        principalTable: "StockRooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonType = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IdCard = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_SupplierId",
                schema: "wh",
                table: "Reservations",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_SupplierId",
                schema: "wh",
                table: "PurchaseRequests",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_KeepCenterId",
                schema: "wh",
                table: "StockFlow",
                column: "KeepCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_OtherKeepCenterId",
                schema: "wh",
                table: "StockFlow",
                column: "OtherKeepCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_OtherStockRoomId",
                schema: "wh",
                table: "StockFlow",
                column: "OtherStockRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_StockRoomId",
                schema: "wh",
                table: "StockFlow",
                column: "StockRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_Suppliers_SupplierId",
                schema: "wh",
                table: "PurchaseRequests",
                column: "SupplierId",
                principalSchema: "wh",
                principalTable: "Suppliers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Suppliers_SupplierId",
                schema: "wh",
                table: "Reservations",
                column: "SupplierId",
                principalSchema: "wh",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_Suppliers_SupplierId",
                schema: "wh",
                table: "PurchaseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Suppliers_SupplierId",
                schema: "wh",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "StockFlow",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Suppliers",
                schema: "wh");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_SupplierId",
                schema: "wh",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequests_SupplierId",
                schema: "wh",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                schema: "wh",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                schema: "wh",
                table: "PurchaseRequests");

            migrationBuilder.RenameColumn(
                name: "OtherPartyType",
                schema: "wh",
                table: "PurchaseRequests",
                newName: "CostCenterType");
        }
    }
}
