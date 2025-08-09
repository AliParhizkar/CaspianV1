using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "wh",
                table: "Sellers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fax",
                schema: "wh",
                table: "Sellers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true)
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
                name: "SellerCategories",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerCategories_SellerCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "wh",
                        principalTable: "SellerCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SellerCategoryMemberships",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerCategoryMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerCategoryMemberships_SellerCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "wh",
                        principalTable: "SellerCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SellerCategoryMemberships_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalSchema: "wh",
                        principalTable: "Sellers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CategoryId",
                schema: "wh",
                table: "ProductCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerCategories_CategoryId",
                schema: "wh",
                table: "SellerCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerCategoryMemberships_CategoryId",
                schema: "wh",
                table: "SellerCategoryMemberships",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerCategoryMemberships_SellerId",
                schema: "wh",
                table: "SellerCategoryMemberships",
                column: "SellerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCategories",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "SellerCategoryMemberships",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "SellerCategories",
                schema: "wh");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "wh",
                table: "Sellers");

            migrationBuilder.DropColumn(
                name: "Fax",
                schema: "wh",
                table: "Sellers");
        }
    }
}
