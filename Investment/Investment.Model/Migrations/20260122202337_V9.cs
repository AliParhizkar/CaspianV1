using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investment.Model.Migrations
{
    /// <inheritdoc />
    public partial class V9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceCompany_AccountingCodes_AccountingCodeId",
                table: "InsuranceCompany");

            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceCompany_Locations_CityId",
                table: "InsuranceCompany");

            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceCompany_Locations_ProvinceId",
                table: "InsuranceCompany");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InsuranceCompany",
                table: "InsuranceCompany");

            migrationBuilder.RenameTable(
                name: "InsuranceCompany",
                newName: "InsuranceCompanies",
                newSchema: "ivm");

            migrationBuilder.RenameIndex(
                name: "IX_InsuranceCompany_ProvinceId",
                schema: "ivm",
                table: "InsuranceCompanies",
                newName: "IX_InsuranceCompanies_ProvinceId");

            migrationBuilder.RenameIndex(
                name: "IX_InsuranceCompany_CityId",
                schema: "ivm",
                table: "InsuranceCompanies",
                newName: "IX_InsuranceCompanies_CityId");

            migrationBuilder.RenameIndex(
                name: "IX_InsuranceCompany_AccountingCodeId",
                schema: "ivm",
                table: "InsuranceCompanies",
                newName: "IX_InsuranceCompanies_AccountingCodeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InsuranceCompanies",
                schema: "ivm",
                table: "InsuranceCompanies",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "InvestmentsCoding",
                schema: "ivm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    DepreciationLawId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentsCoding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestmentsCoding_DepreciationLaws_DepreciationLawId",
                        column: x => x.DepreciationLawId,
                        principalSchema: "ivm",
                        principalTable: "DepreciationLaws",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvestmentsCoding_InvestmentsCoding_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "ivm",
                        principalTable: "InvestmentsCoding",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentsCoding_DepreciationLawId",
                schema: "ivm",
                table: "InvestmentsCoding",
                column: "DepreciationLawId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentsCoding_ParentId",
                schema: "ivm",
                table: "InvestmentsCoding",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceCompanies_AccountingCodes_AccountingCodeId",
                schema: "ivm",
                table: "InsuranceCompanies",
                column: "AccountingCodeId",
                principalSchema: "acc",
                principalTable: "AccountingCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceCompanies_Locations_CityId",
                schema: "ivm",
                table: "InsuranceCompanies",
                column: "CityId",
                principalSchema: "ivm",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceCompanies_Locations_ProvinceId",
                schema: "ivm",
                table: "InsuranceCompanies",
                column: "ProvinceId",
                principalSchema: "ivm",
                principalTable: "Locations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceCompanies_AccountingCodes_AccountingCodeId",
                schema: "ivm",
                table: "InsuranceCompanies");

            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceCompanies_Locations_CityId",
                schema: "ivm",
                table: "InsuranceCompanies");

            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceCompanies_Locations_ProvinceId",
                schema: "ivm",
                table: "InsuranceCompanies");

            migrationBuilder.DropTable(
                name: "InvestmentsCoding",
                schema: "ivm");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InsuranceCompanies",
                schema: "ivm",
                table: "InsuranceCompanies");

            migrationBuilder.RenameTable(
                name: "InsuranceCompanies",
                schema: "ivm",
                newName: "InsuranceCompany");

            migrationBuilder.RenameIndex(
                name: "IX_InsuranceCompanies_ProvinceId",
                table: "InsuranceCompany",
                newName: "IX_InsuranceCompany_ProvinceId");

            migrationBuilder.RenameIndex(
                name: "IX_InsuranceCompanies_CityId",
                table: "InsuranceCompany",
                newName: "IX_InsuranceCompany_CityId");

            migrationBuilder.RenameIndex(
                name: "IX_InsuranceCompanies_AccountingCodeId",
                table: "InsuranceCompany",
                newName: "IX_InsuranceCompany_AccountingCodeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InsuranceCompany",
                table: "InsuranceCompany",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceCompany_AccountingCodes_AccountingCodeId",
                table: "InsuranceCompany",
                column: "AccountingCodeId",
                principalSchema: "acc",
                principalTable: "AccountingCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceCompany_Locations_CityId",
                table: "InsuranceCompany",
                column: "CityId",
                principalSchema: "ivm",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceCompany_Locations_ProvinceId",
                table: "InsuranceCompany",
                column: "ProvinceId",
                principalSchema: "ivm",
                principalTable: "Locations",
                principalColumn: "Id");
        }
    }
}
