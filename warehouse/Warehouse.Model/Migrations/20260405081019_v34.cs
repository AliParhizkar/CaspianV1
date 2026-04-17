using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class v34 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "ActiveStatus",
                schema: "wh",
                table: "DocumentPatterns",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "DefaultBase",
                schema: "wh",
                table: "DocumentPatterns",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "DocumentBases",
                schema: "wh",
                table: "DocumentPatterns",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "DocumentRelationshipInstance",
                schema: "wh",
                table: "DocumentPatterns",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "GoodsType",
                schema: "wh",
                table: "DocumentPatterns",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveStatus",
                schema: "wh",
                table: "DocumentPatterns");

            migrationBuilder.DropColumn(
                name: "DefaultBase",
                schema: "wh",
                table: "DocumentPatterns");

            migrationBuilder.DropColumn(
                name: "DocumentBases",
                schema: "wh",
                table: "DocumentPatterns");

            migrationBuilder.DropColumn(
                name: "DocumentRelationshipInstance",
                schema: "wh",
                table: "DocumentPatterns");

            migrationBuilder.DropColumn(
                name: "GoodsType",
                schema: "wh",
                table: "DocumentPatterns");
        }
    }
}
