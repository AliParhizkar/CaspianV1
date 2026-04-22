using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Procurement.Model.Migrations
{
    /// <inheritdoc />
    public partial class V3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Users_UserId",
                schema: "pcm",
                table: "Suppliers");

            migrationBuilder.AddColumn<byte>(
                name: "CodingLevels",
                schema: "pcm",
                table: "SupplierGroups",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodingLevels",
                schema: "pcm",
                table: "SupplierGroups");
        }
    }
}
