using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketing.Model.Migrations
{
    /// <inheritdoc />
    public partial class Thirteent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "DiscountType",
                schema: "mrk",
                table: "Orders",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentDiscount",
                schema: "mrk",
                table: "Orders",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RoundAmount",
                schema: "mrk",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "RoundType",
                schema: "mrk",
                table: "Orders",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "DiscountType",
                table: "Configs",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentDiscount",
                table: "Configs",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RoundAmount",
                table: "Configs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "RoundType",
                table: "Configs",
                type: "tinyint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountType",
                schema: "mrk",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PercentDiscount",
                schema: "mrk",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RoundAmount",
                schema: "mrk",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RoundType",
                schema: "mrk",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "PercentDiscount",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "RoundAmount",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "RoundType",
                table: "Configs");
        }
    }
}
