using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V29 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Branch",
                schema: "wh",
                table: "Reservations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KeepCenterId",
                schema: "wh",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "ReservationBasis",
                schema: "wh",
                table: "Reservations",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "ReservationType",
                schema: "wh",
                table: "Reservations",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "StockRoomId",
                schema: "wh",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_KeepCenterId",
                schema: "wh",
                table: "Reservations",
                column: "KeepCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_StockRoomId",
                schema: "wh",
                table: "Reservations",
                column: "StockRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_KeepCenters_KeepCenterId",
                schema: "wh",
                table: "Reservations",
                column: "KeepCenterId",
                principalSchema: "wh",
                principalTable: "KeepCenters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_StockRooms_StockRoomId",
                schema: "wh",
                table: "Reservations",
                column: "StockRoomId",
                principalSchema: "wh",
                principalTable: "StockRooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_KeepCenters_KeepCenterId",
                schema: "wh",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_StockRooms_StockRoomId",
                schema: "wh",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_KeepCenterId",
                schema: "wh",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_StockRoomId",
                schema: "wh",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Branch",
                schema: "wh",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "KeepCenterId",
                schema: "wh",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReservationBasis",
                schema: "wh",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReservationType",
                schema: "wh",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "StockRoomId",
                schema: "wh",
                table: "Reservations");
        }
    }
}
