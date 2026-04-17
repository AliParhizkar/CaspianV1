using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V36 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Branch",
                schema: "wh",
                table: "StockFlow",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContractNo",
                schema: "wh",
                table: "StockFlow",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CostCenterId",
                schema: "wh",
                table: "StockFlow",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                schema: "wh",
                table: "StockFlow",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "wh",
                table: "StockFlow",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "No",
                schema: "wh",
                table: "StockFlow",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "OtherPartyType",
                schema: "wh",
                table: "StockFlow",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                schema: "wh",
                table: "StockFlow",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_CostCenterId",
                schema: "wh",
                table: "StockFlow",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_SupplierId",
                schema: "wh",
                table: "StockFlow",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockFlow_CostCenters_CostCenterId",
                schema: "wh",
                table: "StockFlow",
                column: "CostCenterId",
                principalSchema: "acc",
                principalTable: "CostCenters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockFlow_Suppliers_SupplierId",
                schema: "wh",
                table: "StockFlow",
                column: "SupplierId",
                principalSchema: "wh",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockFlow_CostCenters_CostCenterId",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropForeignKey(
                name: "FK_StockFlow_Suppliers_SupplierId",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropIndex(
                name: "IX_StockFlow_CostCenterId",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropIndex(
                name: "IX_StockFlow_SupplierId",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropColumn(
                name: "Branch",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropColumn(
                name: "ContractNo",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropColumn(
                name: "CostCenterId",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropColumn(
                name: "Date",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropColumn(
                name: "No",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropColumn(
                name: "OtherPartyType",
                schema: "wh",
                table: "StockFlow");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                schema: "wh",
                table: "StockFlow");
        }
    }
}
