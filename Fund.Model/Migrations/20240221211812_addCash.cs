using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fund.Model.Migrations
{
    /// <inheritdoc />
    public partial class addCash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Cash");

            migrationBuilder.CreateTable(
                name: "Currencies",
                schema: "Cash",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CashBoxTypes",
                schema: "Cash",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InternationalName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashBoxTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashBoxTypes_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "Cash",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountHolders",
                schema: "Cash",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    BeginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountHolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountHolders_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                schema: "Cash",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FloorLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CashBoxTypeId = table.Column<int>(type: "int", nullable: false),
                    IsReferenceCashBox = table.Column<bool>(type: "bit", nullable: true),
                    AccountHolderId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    TellerType = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountHolders_AccountHolderId",
                        column: x => x.AccountHolderId,
                        principalSchema: "Cash",
                        principalTable: "AccountHolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accounts_CashBoxTypes_CashBoxTypeId",
                        column: x => x.CashBoxTypeId,
                        principalSchema: "Cash",
                        principalTable: "CashBoxTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountHolders_UserId",
                schema: "Cash",
                table: "AccountHolders",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountHolderId",
                schema: "Cash",
                table: "Accounts",
                column: "AccountHolderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CashBoxTypeId",
                schema: "Cash",
                table: "Accounts",
                column: "CashBoxTypeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashBoxTypes_CurrencyId",
                schema: "Cash",
                table: "CashBoxTypes",
                column: "CurrencyId",
                unique: true);

      }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts",
                schema: "Cash");

            migrationBuilder.DropTable(
                name: "ExceptionDetails",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "MenusAccessibility",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "UsersMembership",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "AccountHolders",
                schema: "Cash");

            migrationBuilder.DropTable(
                name: "CashBoxTypes",
                schema: "Cash");

            migrationBuilder.DropTable(
                name: "ExceptionsData",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "Menus",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "Currencies",
                schema: "Cash");

            migrationBuilder.DropTable(
                name: "MenuCategories",
                schema: "cmn");
        }
    }
}
