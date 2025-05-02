using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketing.Model.Migrations
{
    /// <inheritdoc />
    public partial class Eleventh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PriceTotal",
                schema: "mrk",
                table: "OrderDetailToppings",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                computedColumnSql: "[Price] * [Quantity]",
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldComputedColumnSql: "[Id]");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceTotal",
                schema: "mrk",
                table: "OrderDetails",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                computedColumnSql: "([Price] - [Discount]) * [Quantity]",
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldComputedColumnSql: "[Id]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PriceTotal",
                schema: "mrk",
                table: "OrderDetailToppings",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                computedColumnSql: "[Id]",
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldComputedColumnSql: "[Price] * [Quantity]");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceTotal",
                schema: "mrk",
                table: "OrderDetails",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                computedColumnSql: "[Id]",
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldComputedColumnSql: "([Price] - [Discount]) * [Quantity]");
        }
    }
}
