using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "wh");

            migrationBuilder.CreateTable(
                name: "MaterialAddresses",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ParentAddressId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialAddresses_MaterialAddresses_ParentAddressId",
                        column: x => x.ParentAddressId,
                        principalSchema: "wh",
                        principalTable: "MaterialAddresses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaterialUnits",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MeasureType = table.Column<byte>(type: "tinyint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialUnits", x => x.Id);
                });

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
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true)
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
                name: "SimpleData",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DataType = table.Column<byte>(type: "tinyint", nullable: false)
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
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: false)
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
                name: "StockRooms",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StockRoomType = table.Column<byte>(type: "tinyint", nullable: false),
                    FinancialUnitId = table.Column<int>(type: "int", nullable: false),
                    BudgetUnitId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    ScopeId = table.Column<int>(type: "int", nullable: true),
                    PricingMethodType = table.Column<byte>(type: "tinyint", nullable: false),
                    Tel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockRooms_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalSchema: "wh",
                        principalTable: "Scopes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockRooms_SimpleData_BudgetUnitId",
                        column: x => x.BudgetUnitId,
                        principalSchema: "wh",
                        principalTable: "SimpleData",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockRooms_SimpleData_FinancialUnitId",
                        column: x => x.FinancialUnitId,
                        principalSchema: "wh",
                        principalTable: "SimpleData",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockRooms_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Sellers",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SellerType = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EconomicCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RegistrationNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Citizenship = table.Column<byte>(type: "tinyint", nullable: true),
                    IdCard = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Zipcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PreCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tel1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tel2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ProvinceId",
                schema: "wh",
                table: "Cities",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialAddresses_ParentAddressId",
                schema: "wh",
                table: "MaterialAddresses",
                column: "ParentAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Scopes_ParentId",
                schema: "wh",
                table: "Scopes",
                column: "ParentId");

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

            migrationBuilder.CreateIndex(
                name: "IX_StockRooms_BudgetUnitId",
                schema: "wh",
                table: "StockRooms",
                column: "BudgetUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_StockRooms_FinancialUnitId",
                schema: "wh",
                table: "StockRooms",
                column: "FinancialUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_StockRooms_ManagerId",
                schema: "wh",
                table: "StockRooms",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockRooms_ScopeId",
                schema: "wh",
                table: "StockRooms",
                column: "ScopeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialAddresses",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "MaterialUnits",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Sellers",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "StockRooms",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Cities",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Scopes",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "SimpleData",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Provinces",
                schema: "wh");
        }
    }
}
