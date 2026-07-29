using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentAndReceipt.Model.Migrations
{
    /// <inheritdoc />
    public partial class V9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreditLimit",
                schema: "par",
                table: "BankAccount",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReserveBalance",
                schema: "par",
                table: "BankAccount",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Shaba",
                schema: "par",
                table: "BankAccount",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditLimit",
                schema: "par",
                table: "BankAccount");

            migrationBuilder.DropColumn(
                name: "ReserveBalance",
                schema: "par",
                table: "BankAccount");

            migrationBuilder.DropColumn(
                name: "Shaba",
                schema: "par",
                table: "BankAccount");
        }
    }
}
