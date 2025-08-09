using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FinancialUnitId",
                schema: "wh",
                table: "StockRooms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BudgetUnitId",
                schema: "wh",
                table: "StockRooms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                schema: "wh",
                table: "StockRooms",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StockroomId",
                schema: "wh",
                table: "MaterialAddresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialAddresses_StockroomId",
                schema: "wh",
                table: "MaterialAddresses",
                column: "StockroomId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialAddresses_StockRooms_StockroomId",
                schema: "wh",
                table: "MaterialAddresses",
                column: "StockroomId",
                principalSchema: "wh",
                principalTable: "StockRooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialAddresses_StockRooms_StockroomId",
                schema: "wh",
                table: "MaterialAddresses");

            migrationBuilder.DropIndex(
                name: "IX_MaterialAddresses_StockroomId",
                schema: "wh",
                table: "MaterialAddresses");

            migrationBuilder.DropColumn(
                name: "Mobile",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "StockroomId",
                schema: "wh",
                table: "MaterialAddresses");

            migrationBuilder.AlterColumn<int>(
                name: "FinancialUnitId",
                schema: "wh",
                table: "StockRooms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BudgetUnitId",
                schema: "wh",
                table: "StockRooms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
