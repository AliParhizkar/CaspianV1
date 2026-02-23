using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipt_Sellers_SellerId",
                schema: "wh",
                table: "Receipt");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipt_Supplier_SellerId",
                schema: "wh",
                table: "Receipt");

            migrationBuilder.DropForeignKey(
                name: "FK_StockRooms_Scopes_ScopeId",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_StockRooms_SimpleData_SimpleDataId",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_StockRooms_SimpleData_SimpleDataId1",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropTable(
                name: "Scopes",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "SellerCategoryMemberships",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "SimpleData",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "SellerCategories",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Sellers",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Cities",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Provinces",
                schema: "wh");

            migrationBuilder.DropIndex(
                name: "IX_StockRooms_ScopeId",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropIndex(
                name: "IX_StockRooms_SimpleDataId",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropIndex(
                name: "IX_StockRooms_SimpleDataId1",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropIndex(
                name: "IX_Receipt_SellerId",
                schema: "wh",
                table: "Receipt");

            migrationBuilder.DropColumn(
                name: "ScopeId",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "SimpleDataId",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "SimpleDataId1",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "SellerId",
                schema: "wh",
                table: "Receipt");

            migrationBuilder.CreateTable(
                name: "Goods",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodingPatternId = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: false),
                    ProductClassId = table.Column<int>(type: "int", nullable: false),
                    GoodsNature = table.Column<byte>(type: "tinyint", nullable: false),
                    SuspendedInIncoming = table.Column<bool>(type: "bit", nullable: false),
                    SuspendedInOutcoming = table.Column<bool>(type: "bit", nullable: false),
                    ReservationLevel = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goods_CodingPatterns_CodingPatternId",
                        column: x => x.CodingPatternId,
                        principalSchema: "wh",
                        principalTable: "CodingPatterns",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Goods_MeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalSchema: "wh",
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Goods_ProductClasses_ProductClassId",
                        column: x => x.ProductClassId,
                        principalSchema: "wh",
                        principalTable: "ProductClasses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Receipt_SupplierId",
                schema: "wh",
                table: "Receipt",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_CodingPatternId",
                schema: "wh",
                table: "Goods",
                column: "CodingPatternId");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_MeasurementUnitId",
                schema: "wh",
                table: "Goods",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_ProductClassId",
                schema: "wh",
                table: "Goods",
                column: "ProductClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipt_Supplier_SupplierId",
                schema: "wh",
                table: "Receipt",
                column: "SupplierId",
                principalSchema: "wh",
                principalTable: "Supplier",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipt_Supplier_SupplierId",
                schema: "wh",
                table: "Receipt");

            migrationBuilder.DropTable(
                name: "Goods",
                schema: "wh");

            migrationBuilder.DropIndex(
                name: "IX_Receipt_SupplierId",
                schema: "wh",
                table: "Receipt");

            migrationBuilder.AddColumn<int>(
                name: "ScopeId",
                schema: "wh",
                table: "StockRooms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SimpleDataId",
                schema: "wh",
                table: "StockRooms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SimpleDataId1",
                schema: "wh",
                table: "StockRooms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SellerId",
                schema: "wh",
                table: "Receipt",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Provinces",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scopes",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scopes_Scopes_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "wh",
                        principalTable: "Scopes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SellerCategories",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
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
                name: "SimpleData",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataType = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimpleData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalSchema: "wh",
                        principalTable: "Provinces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Sellers",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Citizenship = table.Column<byte>(type: "tinyint", nullable: true),
                    EconomicCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IdCard = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PreCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RegistrationNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SellerType = table.Column<byte>(type: "tinyint", nullable: false),
                    Tel1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tel2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Zipcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sellers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sellers_Cities_CityId",
                        column: x => x.CityId,
                        principalSchema: "wh",
                        principalTable: "Cities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sellers_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalSchema: "wh",
                        principalTable: "Provinces",
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
                name: "IX_StockRooms_ScopeId",
                schema: "wh",
                table: "StockRooms",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockRooms_SimpleDataId",
                schema: "wh",
                table: "StockRooms",
                column: "SimpleDataId");

            migrationBuilder.CreateIndex(
                name: "IX_StockRooms_SimpleDataId1",
                schema: "wh",
                table: "StockRooms",
                column: "SimpleDataId1");

            migrationBuilder.CreateIndex(
                name: "IX_Receipt_SellerId",
                schema: "wh",
                table: "Receipt",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ProvinceId",
                schema: "wh",
                table: "Cities",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Scopes_ParentId",
                schema: "wh",
                table: "Scopes",
                column: "ParentId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_CityId",
                schema: "wh",
                table: "Sellers",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_ProvinceId",
                schema: "wh",
                table: "Sellers",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipt_Sellers_SellerId",
                schema: "wh",
                table: "Receipt",
                column: "SellerId",
                principalSchema: "wh",
                principalTable: "Sellers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipt_Supplier_SellerId",
                schema: "wh",
                table: "Receipt",
                column: "SellerId",
                principalSchema: "wh",
                principalTable: "Supplier",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockRooms_Scopes_ScopeId",
                schema: "wh",
                table: "StockRooms",
                column: "ScopeId",
                principalSchema: "wh",
                principalTable: "Scopes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockRooms_SimpleData_SimpleDataId",
                schema: "wh",
                table: "StockRooms",
                column: "SimpleDataId",
                principalSchema: "wh",
                principalTable: "SimpleData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockRooms_SimpleData_SimpleDataId1",
                schema: "wh",
                table: "StockRooms",
                column: "SimpleDataId1",
                principalSchema: "wh",
                principalTable: "SimpleData",
                principalColumn: "Id");
        }
    }
}
