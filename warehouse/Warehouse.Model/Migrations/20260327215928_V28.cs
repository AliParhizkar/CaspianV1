using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V28 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CostCenterType",
                schema: "wh",
                table: "Reservations",
                newName: "OtherPartyType");

            migrationBuilder.AlterColumn<int>(
                name: "CostCenterId",
                schema: "wh",
                table: "Reservations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "PurchaseRequests",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceDocumentType = table.Column<byte>(type: "tinyint", nullable: false),
                    CostCenterType = table.Column<byte>(type: "tinyint", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    RequisitionerId = table.Column<int>(type: "int", nullable: false),
                    ReceiverKeepCenterId = table.Column<int>(type: "int", nullable: false),
                    ReceiverStockRoomId = table.Column<int>(type: "int", nullable: true),
                    IssuerKeepCenterId = table.Column<int>(type: "int", nullable: false),
                    IssuerStockRoomId = table.Column<int>(type: "int", nullable: true),
                    No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StockRoomDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Loadable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalSchema: "acc",
                        principalTable: "CostCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_KeepCenters_IssuerKeepCenterId",
                        column: x => x.IssuerKeepCenterId,
                        principalSchema: "wh",
                        principalTable: "KeepCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_KeepCenters_ReceiverKeepCenterId",
                        column: x => x.ReceiverKeepCenterId,
                        principalSchema: "wh",
                        principalTable: "KeepCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_StockRooms_IssuerStockRoomId",
                        column: x => x.IssuerStockRoomId,
                        principalSchema: "wh",
                        principalTable: "StockRooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_StockRooms_ReceiverStockRoomId",
                        column: x => x.ReceiverStockRoomId,
                        principalSchema: "wh",
                        principalTable: "StockRooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_Users_RequisitionerId",
                        column: x => x.RequisitionerId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_CostCenterId",
                schema: "wh",
                table: "PurchaseRequests",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_IssuerKeepCenterId",
                schema: "wh",
                table: "PurchaseRequests",
                column: "IssuerKeepCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_IssuerStockRoomId",
                schema: "wh",
                table: "PurchaseRequests",
                column: "IssuerStockRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_ReceiverKeepCenterId",
                schema: "wh",
                table: "PurchaseRequests",
                column: "ReceiverKeepCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_ReceiverStockRoomId",
                schema: "wh",
                table: "PurchaseRequests",
                column: "ReceiverStockRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_RequisitionerId",
                schema: "wh",
                table: "PurchaseRequests",
                column: "RequisitionerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseRequests",
                schema: "wh");

            migrationBuilder.RenameColumn(
                name: "OtherPartyType",
                schema: "wh",
                table: "Reservations",
                newName: "CostCenterType");

            migrationBuilder.AlterColumn<int>(
                name: "CostCenterId",
                schema: "wh",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
