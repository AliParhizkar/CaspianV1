using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentAndReceipt.Model.Migrations
{
    /// <inheritdoc />
    public partial class Eightth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankAccount",
                schema: "par",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    BankAccountTypeId = table.Column<int>(type: "int", nullable: false),
                    BankBranchId = table.Column<int>(type: "int", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccount_BankAccountType_BankAccountTypeId",
                        column: x => x.BankAccountTypeId,
                        principalSchema: "par",
                        principalTable: "BankAccountType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BankAccount_BankBranches_BankBranchId",
                        column: x => x.BankBranchId,
                        principalSchema: "par",
                        principalTable: "BankBranches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BankAccount_Branches_BranchId",
                        column: x => x.BranchId,
                        principalSchema: "par",
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BankAccount_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "par",
                        principalTable: "Currencies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_BankAccountTypeId",
                schema: "par",
                table: "BankAccount",
                column: "BankAccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_BankBranchId",
                schema: "par",
                table: "BankAccount",
                column: "BankBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_BranchId",
                schema: "par",
                table: "BankAccount",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_CurrencyId",
                schema: "par",
                table: "BankAccount",
                column: "CurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankAccount",
                schema: "par");
        }
    }
}
