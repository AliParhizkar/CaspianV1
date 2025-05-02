using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketing.Model.Migrations
{
    /// <inheritdoc />
    public partial class Ninth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ProductToppings",
                newName: "ProductToppings",
                newSchema: "mrk");

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "mrk",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "SettleType",
                schema: "mrk",
                table: "Orders",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                schema: "mrk",
                table: "OrderDetailToppings",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                schema: "mrk",
                table: "OrderDetails",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceTotal",
                schema: "mrk",
                table: "OrderDetailToppings",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                computedColumnSql: "[Id]");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceTotal",
                schema: "mrk",
                table: "OrderDetails",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                computedColumnSql: "[Quantity] * [Price] - [Discount]",
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 2,
                oldComputedColumnSql: "[Quantity] * [Price]");

            migrationBuilder.CreateTable(
                name: "PrinterLocations",
                schema: "mrk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActiveType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrinterLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrinterProduct",
                schema: "mrk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    PrinterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrinterProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrinterProduct_PrinterLocations_PrinterId",
                        column: x => x.PrinterId,
                        principalSchema: "mrk",
                        principalTable: "PrinterLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PrinterProduct_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "mrk",
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrinterProduct_PrinterId",
                schema: "mrk",
                table: "PrinterProduct",
                column: "PrinterId");

            migrationBuilder.CreateIndex(
                name: "IX_PrinterProduct_ProductId",
                schema: "mrk",
                table: "PrinterProduct",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrinterProduct",
                schema: "mrk");

            migrationBuilder.DropTable(
                name: "PrinterLocations",
                schema: "mrk");

            migrationBuilder.DropColumn(
                name: "PriceTotal",
                schema: "mrk",
                table: "OrderDetailToppings");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "mrk",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SettleType",
                schema: "mrk",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "ProductToppings",
                schema: "mrk",
                newName: "ProductToppings");

            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                schema: "mrk",
                table: "OrderDetailToppings",
                type: "float(10)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                schema: "mrk",
                table: "OrderDetails",
                type: "float(10)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<double>(
                name: "PriceTotal",
                schema: "mrk",
                table: "OrderDetails",
                type: "float(10)",
                precision: 10,
                scale: 2,
                nullable: false,
                computedColumnSql: "[Quantity] * [Price]",
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldComputedColumnSql: "[Quantity] * [Price] - [Discount]");
        }
    }
}
