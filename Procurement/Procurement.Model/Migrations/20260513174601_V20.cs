using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Procurement.Model.Migrations
{
    /// <inheritdoc />
    public partial class V20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "InquiryDeadline",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseTypeId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchasingSpecialistId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupplyingUnitId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RequestGoodsSuppliers",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseRequestGoodsId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestGoodsSuppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestGoodsSuppliers_PurchaseRequestsGoods_PurchaseRequestGoodsId",
                        column: x => x.PurchaseRequestGoodsId,
                        principalSchema: "pcm",
                        principalTable: "PurchaseRequestsGoods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequestGoodsSuppliers_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "pcm",
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_PurchaseTypeId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "PurchaseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_PurchasingSpecialistId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "PurchasingSpecialistId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_SupplierId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_SupplyingUnitId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "SupplyingUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestGoodsSuppliers_PurchaseRequestGoodsId",
                schema: "pcm",
                table: "RequestGoodsSuppliers",
                column: "PurchaseRequestGoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestGoodsSuppliers_SupplierId",
                schema: "pcm",
                table: "RequestGoodsSuppliers",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequestsGoods_PurchaseTypes_PurchaseTypeId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "PurchaseTypeId",
                principalSchema: "pcm",
                principalTable: "PurchaseTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequestsGoods_PurchasingSpecialist_PurchasingSpecialistId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "PurchasingSpecialistId",
                principalSchema: "pcm",
                principalTable: "PurchasingSpecialist",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequestsGoods_Suppliers_SupplierId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "SupplierId",
                principalSchema: "pcm",
                principalTable: "Suppliers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequestsGoods_SupplyingUnits_SupplyingUnitId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "SupplyingUnitId",
                principalSchema: "pcm",
                principalTable: "SupplyingUnits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequestsGoods_PurchaseTypes_PurchaseTypeId",
                schema: "pcm",
                table: "PurchaseRequestsGoods");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequestsGoods_PurchasingSpecialist_PurchasingSpecialistId",
                schema: "pcm",
                table: "PurchaseRequestsGoods");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequestsGoods_Suppliers_SupplierId",
                schema: "pcm",
                table: "PurchaseRequestsGoods");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequestsGoods_SupplyingUnits_SupplyingUnitId",
                schema: "pcm",
                table: "PurchaseRequestsGoods");

            migrationBuilder.DropTable(
                name: "RequestGoodsSuppliers",
                schema: "pcm");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequestsGoods_PurchaseTypeId",
                schema: "pcm",
                table: "PurchaseRequestsGoods");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequestsGoods_PurchasingSpecialistId",
                schema: "pcm",
                table: "PurchaseRequestsGoods");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequestsGoods_SupplierId",
                schema: "pcm",
                table: "PurchaseRequestsGoods");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequestsGoods_SupplyingUnitId",
                schema: "pcm",
                table: "PurchaseRequestsGoods");

            migrationBuilder.DropColumn(
                name: "InquiryDeadline",
                schema: "pcm",
                table: "PurchaseRequestsGoods");

            migrationBuilder.DropColumn(
                name: "PurchaseTypeId",
                schema: "pcm",
                table: "PurchaseRequestsGoods");

            migrationBuilder.DropColumn(
                name: "PurchasingSpecialistId",
                schema: "pcm",
                table: "PurchaseRequestsGoods");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                schema: "pcm",
                table: "PurchaseRequestsGoods");

            migrationBuilder.DropColumn(
                name: "SupplyingUnitId",
                schema: "pcm",
                table: "PurchaseRequestsGoods");
        }
    }
}
