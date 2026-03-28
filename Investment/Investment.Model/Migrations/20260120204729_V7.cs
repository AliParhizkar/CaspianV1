using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investment.Model.Migrations
{
    /// <inheritdoc />
    public partial class V7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceCompany_AccountingCodes_AccountingCodeId",
                table: "InsuranceCompany");

            migrationBuilder.EnsureSchema(
                name: "acc");

            migrationBuilder.CreateTable(
                name: "DepreciationLawGroups",
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
                    table.PrimaryKey("PK_DepreciationLawGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepreciationLaws",
                schema: "ivm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepreciationLawGroupId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CalculateMethod = table.Column<byte>(type: "tinyint", nullable: false),
                    Rate = table.Column<int>(type: "int", nullable: false),
                    UsefulLife = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepreciationLaws", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepreciationLaws_DepreciationLawGroups_DepreciationLawGroupId",
                        column: x => x.DepreciationLawGroupId,
                        principalSchema: "ivm",
                        principalTable: "DepreciationLawGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepreciationLaws_DepreciationLawGroupId",
                schema: "ivm",
                table: "DepreciationLaws",
                column: "DepreciationLawGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceCompany_AccountingCodes_AccountingCodeId",
                table: "InsuranceCompany",
                column: "AccountingCodeId",
                principalSchema: "acc",
                principalTable: "AccountingCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceCompany_AccountingCodes_AccountingCodeId",
                table: "InsuranceCompany");

            migrationBuilder.DropTable(
                name: "DepreciationLaws",
                schema: "ivm");

            migrationBuilder.DropTable(
                name: "DepreciationLawGroups",
                schema: "ivm");

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceCompany_AccountingCodes_AccountingCodeId",
                table: "InsuranceCompany",
                column: "AccountingCodeId",
                principalSchema: "acc",
                principalTable: "AccountingCodes",
                principalColumn: "Id");
        }
    }
}
