using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Model.Migrations
{
    /// <inheritdoc />
    public partial class Version2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpsertDate",
                schema: "demo",
                table: "Countries ",
                type: "datetime(2)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                schema: "demo",
                table: "Countries ",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Countries _UserId",
                schema: "demo",
                table: "Countries ",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Countries _Users_UserId",
                schema: "demo",
                table: "Countries ",
                column: "UserId",
                principalSchema: "cmn",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Countries _Users_UserId",
                schema: "demo",
                table: "Countries ");

            migrationBuilder.DropIndex(
                name: "IX_Countries _UserId",
                schema: "demo",
                table: "Countries ");

            migrationBuilder.DropColumn(
                name: "UpsertDate",
                schema: "demo",
                table: "Countries ");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "demo",
                table: "Countries ");
        }
    }
}
