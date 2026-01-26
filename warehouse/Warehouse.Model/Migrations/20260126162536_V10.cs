using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockRooms_SimpleData_BudgetUnitId",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_StockRooms_SimpleData_FinancialUnitId",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropTable(
                name: "MaterialAddresses",
                schema: "wh");

            migrationBuilder.DropColumn(
                name: "Address",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "Mobile",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "PricingMethodType",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "StockRoomType",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "Tel",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.RenameColumn(
                name: "FinancialUnitId",
                schema: "wh",
                table: "StockRooms",
                newName: "SimpleDataId1");

            migrationBuilder.RenameColumn(
                name: "BudgetUnitId",
                schema: "wh",
                table: "StockRooms",
                newName: "SimpleDataId");

            migrationBuilder.RenameIndex(
                name: "IX_StockRooms_FinancialUnitId",
                schema: "wh",
                table: "StockRooms",
                newName: "IX_StockRooms_SimpleDataId1");

            migrationBuilder.RenameIndex(
                name: "IX_StockRooms_BudgetUnitId",
                schema: "wh",
                table: "StockRooms",
                newName: "IX_StockRooms_SimpleDataId");

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                schema: "wh",
                table: "StockRooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KeepCenterId",
                schema: "wh",
                table: "StockRooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "MemberInManyParts",
                schema: "wh",
                table: "StockRooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MemberOfMiddlePart",
                schema: "wh",
                table: "StockRooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Branches",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LocationType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Locations_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "wh",
                        principalTable: "Locations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaterialLocations",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StockroomId = table.Column<int>(type: "int", nullable: false),
                    ParentLocationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialLocations_MaterialLocations_ParentLocationId",
                        column: x => x.ParentLocationId,
                        principalSchema: "wh",
                        principalTable: "MaterialLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaterialLocations_StockRooms_StockroomId",
                        column: x => x.StockroomId,
                        principalSchema: "wh",
                        principalTable: "StockRooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KeepCenters",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    Tell = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeepCenters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeepCenters_Branches_BranchId",
                        column: x => x.BranchId,
                        principalSchema: "wh",
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KeepCenters_Locations_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "wh",
                        principalTable: "Locations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockRooms_BranchId",
                schema: "wh",
                table: "StockRooms",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_StockRooms_KeepCenterId",
                schema: "wh",
                table: "StockRooms",
                column: "KeepCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_KeepCenters_BranchId",
                schema: "wh",
                table: "KeepCenters",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_KeepCenters_LocationId",
                schema: "wh",
                table: "KeepCenters",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ParentId",
                schema: "wh",
                table: "Locations",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialLocations_ParentLocationId",
                schema: "wh",
                table: "MaterialLocations",
                column: "ParentLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialLocations_StockroomId",
                schema: "wh",
                table: "MaterialLocations",
                column: "StockroomId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockRooms_Branches_BranchId",
                schema: "wh",
                table: "StockRooms",
                column: "BranchId",
                principalSchema: "wh",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockRooms_KeepCenters_KeepCenterId",
                schema: "wh",
                table: "StockRooms",
                column: "KeepCenterId",
                principalSchema: "wh",
                principalTable: "KeepCenters",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockRooms_Branches_BranchId",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_StockRooms_KeepCenters_KeepCenterId",
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
                name: "KeepCenters",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "MaterialLocations",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Branches",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Locations",
                schema: "wh");

            migrationBuilder.DropIndex(
                name: "IX_StockRooms_BranchId",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropIndex(
                name: "IX_StockRooms_KeepCenterId",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "KeepCenterId",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "MemberInManyParts",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "MemberOfMiddlePart",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.RenameColumn(
                name: "SimpleDataId1",
                schema: "wh",
                table: "StockRooms",
                newName: "FinancialUnitId");

            migrationBuilder.RenameColumn(
                name: "SimpleDataId",
                schema: "wh",
                table: "StockRooms",
                newName: "BudgetUnitId");

            migrationBuilder.RenameIndex(
                name: "IX_StockRooms_SimpleDataId1",
                schema: "wh",
                table: "StockRooms",
                newName: "IX_StockRooms_FinancialUnitId");

            migrationBuilder.RenameIndex(
                name: "IX_StockRooms_SimpleDataId",
                schema: "wh",
                table: "StockRooms",
                newName: "IX_StockRooms_BudgetUnitId");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "wh",
                table: "StockRooms",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "wh",
                table: "StockRooms",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                schema: "wh",
                table: "StockRooms",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "PricingMethodType",
                schema: "wh",
                table: "StockRooms",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "StockRoomType",
                schema: "wh",
                table: "StockRooms",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "Tel",
                schema: "wh",
                table: "StockRooms",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MaterialAddresses",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentAddressId = table.Column<int>(type: "int", nullable: true),
                    StockroomId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
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
                    table.ForeignKey(
                        name: "FK_MaterialAddresses_StockRooms_StockroomId",
                        column: x => x.StockroomId,
                        principalSchema: "wh",
                        principalTable: "StockRooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialAddresses_ParentAddressId",
                schema: "wh",
                table: "MaterialAddresses",
                column: "ParentAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialAddresses_StockroomId",
                schema: "wh",
                table: "MaterialAddresses",
                column: "StockroomId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockRooms_SimpleData_BudgetUnitId",
                schema: "wh",
                table: "StockRooms",
                column: "BudgetUnitId",
                principalSchema: "wh",
                principalTable: "SimpleData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockRooms_SimpleData_FinancialUnitId",
                schema: "wh",
                table: "StockRooms",
                column: "FinancialUnitId",
                principalSchema: "wh",
                principalTable: "SimpleData",
                principalColumn: "Id");
        }
    }
}
