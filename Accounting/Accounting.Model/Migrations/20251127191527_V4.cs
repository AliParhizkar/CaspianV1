using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Model.Migrations
{
    /// <inheritdoc />
    public partial class V4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "CodingLevelType",
                schema: "acc",
                table: "AccountingCodes",
                type: "tinyint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodingLevelType",
                schema: "acc",
                table: "AccountingCodes");
        }
    }
}
