using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Engine.Model.Migrations
{
    /// <inheritdoc />
    public partial class V3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                schema: "cmn",
                table: "Menus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                schema: "cmn",
                table: "Menus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
