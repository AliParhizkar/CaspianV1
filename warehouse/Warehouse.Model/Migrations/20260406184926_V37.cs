using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V37 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseRequestsGoods",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "PurchaseRequests",
                schema: "wh");

            migrationBuilder.CreateTable(
                name: "GoodsRequests",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceDocumentType = table.Column<byte>(type: "tinyint", nullable: false),
                    OtherPartyType = table.Column<byte>(type: "tinyint", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    RequisitionerId = table.Column<int>(type: "int", nullable: false),
                    ReceiverKeepCenterId = table.Column<int>(type: "int", nullable: false),
                    ReceiverStockRoomId = table.Column<int>(type: "int", nullable: true),
                    IssuerKeepCenterId = table.Column<int>(type: "int", nullable: false),
                    IssuerStockRoomId = table.Column<int>(type: "int", nullable: true),
                    No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StockRoomDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Loadable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsRequests_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalSchema: "acc",
                        principalTable: "CostCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodsRequests_KeepCenters_IssuerKeepCenterId",
                        column: x => x.IssuerKeepCenterId,
                        principalSchema: "wh",
                        principalTable: "KeepCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodsRequests_KeepCenters_ReceiverKeepCenterId",
                        column: x => x.ReceiverKeepCenterId,
                        principalSchema: "wh",
                        principalTable: "KeepCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodsRequests_StockRooms_IssuerStockRoomId",
                        column: x => x.IssuerStockRoomId,
                        principalSchema: "wh",
                        principalTable: "StockRooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodsRequests_StockRooms_ReceiverStockRoomId",
                        column: x => x.ReceiverStockRoomId,
                        principalSchema: "wh",
                        principalTable: "StockRooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodsRequests_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "wh",
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodsRequests_Users_RequisitionerId",
                        column: x => x.RequisitionerId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestsGoods",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsRequestId = table.Column<int>(type: "int", nullable: false),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float(15)", precision: 15, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestsGoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestsGoods_GoodsRequests_GoodsRequestId",
                        column: x => x.GoodsRequestId,
                        principalSchema: "wh",
                        principalTable: "GoodsRequests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequestsGoods_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsRequests_CostCenterId",
                schema: "wh",
                table: "GoodsRequests",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsRequests_IssuerKeepCenterId",
                schema: "wh",
                table: "GoodsRequests",
                column: "IssuerKeepCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsRequests_IssuerStockRoomId",
                schema: "wh",
                table: "GoodsRequests",
                column: "IssuerStockRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsRequests_ReceiverKeepCenterId",
                schema: "wh",
                table: "GoodsRequests",
                column: "ReceiverKeepCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsRequests_ReceiverStockRoomId",
                schema: "wh",
                table: "GoodsRequests",
                column: "ReceiverStockRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsRequests_RequisitionerId",
                schema: "wh",
                table: "GoodsRequests",
                column: "RequisitionerId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsRequests_SupplierId",
                schema: "wh",
                table: "GoodsRequests",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestsGoods_GoodsId",
                schema: "wh",
                table: "RequestsGoods",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestsGoods_GoodsRequestId",
                schema: "wh",
                table: "RequestsGoods",
                column: "GoodsRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestsGoods",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "GoodsRequests",
                schema: "wh");

            migrationBuilder.CreateTable(
                name: "PurchaseRequests",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    IssuerKeepCenterId = table.Column<int>(type: "int", nullable: false),
                    IssuerStockRoomId = table.Column<int>(type: "int", nullable: true),
                    ReceiverKeepCenterId = table.Column<int>(type: "int", nullable: false),
                    ReceiverStockRoomId = table.Column<int>(type: "int", nullable: true),
                    RequisitionerId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Loadable = table.Column<bool>(type: "bit", nullable: false),
                    No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OtherPartyType = table.Column<byte>(type: "tinyint", nullable: false),
                    ReferenceDocumentType = table.Column<byte>(type: "tinyint", nullable: false),
                    StockRoomDate = table.Column<DateOnly>(type: "date", nullable: false)
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
                        name: "FK_PurchaseRequests_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "wh",
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_Users_RequisitionerId",
                        column: x => x.RequisitionerId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequestsGoods",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    PurchaseRequestId = table.Column<int>(type: "int", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_SupplierId",
                schema: "wh",
                table: "PurchaseRequests",
                column: "SupplierId");

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
        }
    }
}
