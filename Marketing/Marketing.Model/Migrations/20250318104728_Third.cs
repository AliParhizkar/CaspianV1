using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
//185.18.213.248\SQLEXPRESS
namespace Marketing.Model.Migrations
{
    /// <inheritdoc />
    public partial class Third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "mrk",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Configs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                schema: "mrk",
                table: "CustomersAddress",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                schema: "mrk",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                schema: "mrk",
                table: "CustomersAddress"); 
        }
    }
}
