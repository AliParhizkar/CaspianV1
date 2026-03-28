using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investment.Model.Migrations
{
    /// <inheritdoc />
    public partial class V6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exchanges",
                schema: "ivm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exchanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceCompany",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccountingCodeId = table.Column<int>(type: "int", nullable: false),
                    InsuranceCompanyType = table.Column<byte>(type: "tinyint", nullable: false),
                    NationalCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EconomicCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    Tell = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceCompany", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceCompany_AccountingCodes_AccountingCodeId",
                        column: x => x.AccountingCodeId,
                        principalSchema: "acc",
                        principalTable: "AccountingCodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InsuranceCompany_Locations_CityId",
                        column: x => x.CityId,
                        principalSchema: "ivm",
                        principalTable: "Locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InsuranceCompany_Locations_ProvinceId",
                        column: x => x.ProvinceId,
                        principalSchema: "ivm",
                        principalTable: "Locations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvestmentUnitAccess",
                schema: "ivm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    InvestmentUnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentUnitAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestmentUnitAccess_InvestmentUnits_InvestmentUnitId",
                        column: x => x.InvestmentUnitId,
                        principalSchema: "ivm",
                        principalTable: "InvestmentUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvestmentUnitAccess_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                schema: "ivm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExchangeId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Rate = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Exchanges_ExchangeId",
                        column: x => x.ExchangeId,
                        principalSchema: "ivm",
                        principalTable: "Exchanges",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_ExchangeId",
                schema: "ivm",
                table: "ExchangeRates",
                column: "ExchangeId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceCompany_AccountingCodeId",
                table: "InsuranceCompany",
                column: "AccountingCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceCompany_CityId",
                table: "InsuranceCompany",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceCompany_ProvinceId",
                table: "InsuranceCompany",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentUnitAccess_InvestmentUnitId",
                schema: "ivm",
                table: "InvestmentUnitAccess",
                column: "InvestmentUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentUnitAccess_UserId",
                schema: "ivm",
                table: "InvestmentUnitAccess",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeRates",
                schema: "ivm");

            migrationBuilder.DropTable(
                name: "InsuranceCompany");

            migrationBuilder.DropTable(
                name: "InvestmentUnitAccess",
                schema: "ivm");

            migrationBuilder.DropTable(
                name: "Exchanges",
                schema: "ivm");
        }
    }
}
