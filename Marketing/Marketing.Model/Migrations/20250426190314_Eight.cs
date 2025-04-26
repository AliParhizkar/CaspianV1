using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketing.Model.Migrations
{
    /// <inheritdoc />
    public partial class Eight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Toppings",
                schema: "mrk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Price = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Toppings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetailToppings",
                schema: "mrk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDetailId = table.Column<int>(type: "int", nullable: false),
                    ToppingId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetailToppings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetailToppings_OrderDetails_OrderDetailId",
                        column: x => x.OrderDetailId,
                        principalSchema: "mrk",
                        principalTable: "OrderDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderDetailToppings_Toppings_ToppingId",
                        column: x => x.ToppingId,
                        principalSchema: "mrk",
                        principalTable: "Toppings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductToppings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ToppingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductToppings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductToppings_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "mrk",
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductToppings_Toppings_ToppingId",
                        column: x => x.ToppingId,
                        principalSchema: "mrk",
                        principalTable: "Toppings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailToppings_OrderDetailId",
                schema: "mrk",
                table: "OrderDetailToppings",
                column: "OrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailToppings_ToppingId",
                schema: "mrk",
                table: "OrderDetailToppings",
                column: "ToppingId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductToppings_ProductId",
                table: "ProductToppings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductToppings_ToppingId",
                table: "ProductToppings",
                column: "ToppingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetailToppings",
                schema: "mrk");

            migrationBuilder.DropTable(
                name: "ProductToppings");

            migrationBuilder.DropTable(
                name: "Toppings",
                schema: "mrk");
        }
    }
}
