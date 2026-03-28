using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyList_GoodsProperties_GoodsPropertyId",
                table: "PropertyList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertyList",
                table: "PropertyList");

            migrationBuilder.RenameTable(
                name: "PropertyList",
                newName: "PropertiesList",
                newSchema: "wh");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyList_GoodsPropertyId",
                schema: "wh",
                table: "PropertiesList",
                newName: "IX_PropertiesList_GoodsPropertyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertiesList",
                schema: "wh",
                table: "PropertiesList",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertiesList_GoodsProperties_GoodsPropertyId",
                schema: "wh",
                table: "PropertiesList",
                column: "GoodsPropertyId",
                principalSchema: "wh",
                principalTable: "GoodsProperties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertiesList_GoodsProperties_GoodsPropertyId",
                schema: "wh",
                table: "PropertiesList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertiesList",
                schema: "wh",
                table: "PropertiesList");

            migrationBuilder.RenameTable(
                name: "PropertiesList",
                schema: "wh",
                newName: "PropertyList");

            migrationBuilder.RenameIndex(
                name: "IX_PropertiesList_GoodsPropertyId",
                table: "PropertyList",
                newName: "IX_PropertyList_GoodsPropertyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertyList",
                table: "PropertyList",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyList_GoodsProperties_GoodsPropertyId",
                table: "PropertyList",
                column: "GoodsPropertyId",
                principalSchema: "wh",
                principalTable: "GoodsProperties",
                principalColumn: "Id");
        }
    }
}
