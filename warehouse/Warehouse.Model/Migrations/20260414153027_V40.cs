using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V40 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasOtherParty",
                schema: "wh",
                table: "DocumentPatterns",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasOtherParty",
                schema: "wh",
                table: "DocumentPatterns");
        }
    }
}
