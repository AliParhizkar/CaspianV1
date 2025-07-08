using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Model.Migrations
{
    /// <inheritdoc />
    public partial class V3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ZipCode",
                schema: "demo",
                table: "CustomersAddresses",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "demo",
                table: "CustomersAddresses",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpsertDate",
                schema: "demo",
                table: "Cities",
                type: "datetime2(2)",
                precision: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpsertUserId",
                schema: "demo",
                table: "Cities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_UpsertUserId",
                schema: "demo",
                table: "Cities",
                column: "UpsertUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Users_UpsertUserId",
                schema: "demo",
                table: "Cities",
                column: "UpsertUserId",
                principalSchema: "cmn",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Users_UpsertUserId",
                schema: "demo",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_UpsertUserId",
                schema: "demo",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "UpsertDate",
                schema: "demo",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "UpsertUserId",
                schema: "demo",
                table: "Cities");

            migrationBuilder.AlterColumn<string>(
                name: "ZipCode",
                schema: "demo",
                table: "CustomersAddresses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "demo",
                table: "CustomersAddresses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
