using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Procurement.Model.Migrations
{
    /// <inheritdoc />
    public partial class V15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseRequests",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ProcurementItemType = table.Column<byte>(type: "tinyint", nullable: false),
                    KeepCenterId = table.Column<int>(type: "int", nullable: false),
                    RequesterId = table.Column<int>(type: "int", nullable: true),
                    OtherPartyType = table.Column<byte>(type: "tinyint", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PurchaseRequestType = table.Column<byte>(type: "tinyint", nullable: true)
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
                        name: "FK_PurchaseRequests_KeepCenters_KeepCenterId",
                        column: x => x.KeepCenterId,
                        principalSchema: "wh",
                        principalTable: "KeepCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "pcm",
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_Users_RequesterId",
                        column: x => x.RequesterId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UrgentPurchases",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrgentPurchases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequestsGoods",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseRequestId = table.Column<int>(type: "int", nullable: false),
                    ReferenceDocumentType = table.Column<byte>(type: "tinyint", nullable: false),
                    GoodsFlowId = table.Column<int>(type: "int", nullable: true),
                    ProcurementItemId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    DateNeed = table.Column<DateOnly>(type: "date", nullable: false),
                    UrgentPurchaseId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequestsGoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestsGoods_GoodsFlow_GoodsFlowId",
                        column: x => x.GoodsFlowId,
                        principalSchema: "wh",
                        principalTable: "GoodsFlow",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequestsGoods_ProcurementItem_ProcurementItemId",
                        column: x => x.ProcurementItemId,
                        principalSchema: "pcm",
                        principalTable: "ProcurementItem",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequestsGoods_PurchaseRequests_PurchaseRequestId",
                        column: x => x.PurchaseRequestId,
                        principalSchema: "pcm",
                        principalTable: "PurchaseRequests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequestsGoods_UrgentPurchases_UrgentPurchaseId",
                        column: x => x.UrgentPurchaseId,
                        principalSchema: "pcm",
                        principalTable: "UrgentPurchases",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_CostCenterId",
                schema: "pcm",
                table: "PurchaseRequests",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_KeepCenterId",
                schema: "pcm",
                table: "PurchaseRequests",
                column: "KeepCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_RequesterId",
                schema: "pcm",
                table: "PurchaseRequests",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_SupplierId",
                schema: "pcm",
                table: "PurchaseRequests",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_GoodsFlowId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "GoodsFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_ProcurementItemId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "ProcurementItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_PurchaseRequestId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "PurchaseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_UrgentPurchaseId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "UrgentPurchaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseRequestsGoods",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "PurchaseRequests",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "UrgentPurchases",
                schema: "pcm");
        }
    }
}
