using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Model.Migrations
{
    /// <inheritdoc />
    public partial class V8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "AccountType",
                schema: "acc",
                table: "AccountingCodes",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AccountType",
                schema: "acc",
                table: "AccountingCodes",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }
    }
}
