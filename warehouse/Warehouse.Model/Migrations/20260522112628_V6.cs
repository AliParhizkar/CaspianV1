using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BooleanField",
                schema: "wh",
                table: "PropertiesOfGoods",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateField",
                schema: "wh",
                table: "PropertiesOfGoods",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NumericField",
                schema: "wh",
                table: "PropertiesOfGoods",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PropertyListIdField",
                schema: "wh",
                table: "PropertiesOfGoods",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StringField",
                schema: "wh",
                table: "PropertiesOfGoods",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertiesOfGoods_PropertyListIdField",
                schema: "wh",
                table: "PropertiesOfGoods",
                column: "PropertyListIdField");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertiesOfGoods_PropertiesList_PropertyListIdField",
                schema: "wh",
                table: "PropertiesOfGoods",
                column: "PropertyListIdField",
                principalSchema: "wh",
                principalTable: "PropertiesList",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertiesOfGoods_PropertiesList_PropertyListIdField",
                schema: "wh",
                table: "PropertiesOfGoods");

            migrationBuilder.DropIndex(
                name: "IX_PropertiesOfGoods_PropertyListIdField",
                schema: "wh",
                table: "PropertiesOfGoods");

            migrationBuilder.DropColumn(
                name: "BooleanField",
                schema: "wh",
                table: "PropertiesOfGoods");

            migrationBuilder.DropColumn(
                name: "DateField",
                schema: "wh",
                table: "PropertiesOfGoods");

            migrationBuilder.DropColumn(
                name: "NumericField",
                schema: "wh",
                table: "PropertiesOfGoods");

            migrationBuilder.DropColumn(
                name: "PropertyListIdField",
                schema: "wh",
                table: "PropertiesOfGoods");

            migrationBuilder.DropColumn(
                name: "StringField",
                schema: "wh",
                table: "PropertiesOfGoods");
        }
    }
}
