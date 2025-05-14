using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketing.Model.Migrations
{
    /// <inheritdoc />
    public partial class V27 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CashierId",
                schema: "mrk",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpsertDate",
                schema: "mrk",
                table: "Orders",
                type: "datetime2(2)",
                precision: 2,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UpsertUserId",
                schema: "mrk",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Cashier",
                schema: "mrk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Gender = table.Column<byte>(type: "tinyint", nullable: false),
                    RelatedUserId = table.Column<int>(type: "int", nullable: false),
                    ActiveType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cashier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cashier_Users_RelatedUserId",
                        column: x => x.RelatedUserId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CashierId",
                schema: "mrk",
                table: "Orders",
                column: "CashierId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UpsertUserId",
                schema: "mrk",
                table: "Orders",
                column: "UpsertUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cashier_RelatedUserId",
                schema: "mrk",
                table: "Cashier",
                column: "RelatedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Cashier_CashierId",
                schema: "mrk",
                table: "Orders",
                column: "CashierId",
                principalSchema: "mrk",
                principalTable: "Cashier",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UpsertUserId",
                schema: "mrk",
                table: "Orders",
                column: "UpsertUserId",
                principalSchema: "cmn",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Cashier_CashierId",
                schema: "mrk",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UpsertUserId",
                schema: "mrk",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Cashier",
                schema: "mrk");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CashierId",
                schema: "mrk",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UpsertUserId",
                schema: "mrk",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CashierId",
                schema: "mrk",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UpsertDate",
                schema: "mrk",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UpsertUserId",
                schema: "mrk",
                table: "Orders");
        }
    }
}
