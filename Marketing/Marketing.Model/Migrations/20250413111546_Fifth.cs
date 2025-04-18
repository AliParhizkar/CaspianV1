using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketing.Model.Migrations
{
    /// <inheritdoc />
    public partial class Fifth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DefaultAddressManagment",
                table: "Configs",
                newName: "DefaultAddressManagement");

            migrationBuilder.AddColumn<byte>(
                name: "CategoryType",
                table: "Configs",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryType",
                table: "Configs");

            migrationBuilder.RenameColumn(
                name: "DefaultAddressManagement",
                table: "Configs",
                newName: "DefaultAddressManagment");
        }
    }
}
