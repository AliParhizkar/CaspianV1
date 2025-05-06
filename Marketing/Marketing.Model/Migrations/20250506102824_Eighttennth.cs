using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketing.Model.Migrations
{
    /// <inheritdoc />
    public partial class Eighttennth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecialCustomerPrice",
                schema: "mrk",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TakeOutPrice",
                schema: "mrk",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "OrderType",
                schema: "mrk",
                table: "Orders",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSpecial",
                schema: "mrk",
                table: "Customer",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecialCustomerPrice",
                schema: "mrk",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TakeOutPrice",
                schema: "mrk",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OrderType",
                schema: "mrk",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsSpecial",
                schema: "mrk",
                table: "Customer");
        }
    }
}
