using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductDescriptions",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "ReceiptDetails",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Receipt",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "MaterialUnits",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "ProductCategories",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Supplier",
                schema: "wh");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                schema: "wh",
                table: "StandardChangeUnits",
                type: "decimal(10,3)",
                precision: 10,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                schema: "wh",
                table: "StandardChangeUnits",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,3)",
                oldPrecision: 10,
                oldScale: 3);

            migrationBuilder.CreateTable(
                name: "MaterialUnits",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MeasureType = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HasChild = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCategories_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "wh",
                        principalTable: "ProductCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RelatedUserId = table.Column<int>(type: "int", nullable: false),
                    IdCard = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ParentName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Supplier_Users_RelatedUserId",
                        column: x => x.RelatedUserId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    MaterialUnitId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Maximum = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OrderPoint = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    ProductType = table.Column<byte>(type: "tinyint", nullable: true),
                    StandardCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StandardPrice = table.Column<int>(type: "int", nullable: true),
                    TechnicalCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WarningDate = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_MaterialUnits_MaterialUnitId",
                        column: x => x.MaterialUnitId,
                        principalSchema: "wh",
                        principalTable: "MaterialUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "wh",
                        principalTable: "ProductCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Receipt",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReceiptDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ReceiptType = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receipt_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "wh",
                        principalTable: "Supplier",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductDescriptions",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    DescriptionType = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDescriptions_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReceiptDetails",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ReceiptId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpireDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptDetails_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReceiptDetails_Receipt_ReceiptId",
                        column: x => x.ReceiptId,
                        principalSchema: "wh",
                        principalTable: "Receipt",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryId",
                table: "Product",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_MaterialUnitId",
                table: "Product",
                column: "MaterialUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CategoryId",
                schema: "wh",
                table: "ProductCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDescriptions_ProductId",
                schema: "wh",
                table: "ProductDescriptions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipt_SupplierId",
                schema: "wh",
                table: "Receipt",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDetails_ProductId",
                schema: "wh",
                table: "ReceiptDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDetails_ReceiptId",
                schema: "wh",
                table: "ReceiptDetails",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_RelatedUserId",
                schema: "wh",
                table: "Supplier",
                column: "RelatedUserId");
        }
    }
}
