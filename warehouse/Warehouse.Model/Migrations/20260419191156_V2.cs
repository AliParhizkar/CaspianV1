using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockRooms_Scopes_ScopeId",
                schema: "wh",
                table: "StockRooms");

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

            migrationBuilder.DropTable(
                name: "MaterialUnits",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Scopes",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Sellers",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "SimpleData",
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

            migrationBuilder.DropColumn(
                name: "Address",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "PricingMethodType",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "ScopeId",
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
                newName: "KeepCenterId");

            migrationBuilder.RenameColumn(
                name: "BudgetUnitId",
                schema: "wh",
                table: "StockRooms",
                newName: "BranchId");

            migrationBuilder.RenameIndex(
                name: "IX_StockRooms_FinancialUnitId",
                schema: "wh",
                table: "StockRooms",
                newName: "IX_StockRooms_KeepCenterId");

            migrationBuilder.RenameIndex(
                name: "IX_StockRooms_BudgetUnitId",
                schema: "wh",
                table: "StockRooms",
                newName: "IX_StockRooms_BranchId");

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
                name: "BarcodePatterns",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarcodePatterns", x => x.Id);
                });

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
                name: "DocumentPatterns",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DocumentType = table.Column<byte>(type: "tinyint", nullable: false),
                    PurchaseType = table.Column<byte>(type: "tinyint", nullable: true),
                    InOutType = table.Column<byte>(type: "tinyint", nullable: false),
                    InventoryImpactType = table.Column<byte>(type: "tinyint", nullable: false),
                    RowsCount = table.Column<int>(type: "int", nullable: false),
                    FromStockRoomFieldName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HasToStockRoom = table.Column<bool>(type: "bit", nullable: false),
                    ToStockRoomFieldName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HasReceiverIssuer = table.Column<bool>(type: "bit", nullable: false),
                    ReceiverIssuerFieldName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DocumentRelationshipType = table.Column<byte>(type: "tinyint", nullable: false),
                    DocumentRelationshipInstance = table.Column<byte>(type: "tinyint", nullable: false),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    DocumentBases = table.Column<byte>(type: "tinyint", nullable: false),
                    DefaultBase = table.Column<byte>(type: "tinyint", nullable: false),
                    GoodsType = table.Column<byte>(type: "tinyint", nullable: false),
                    HasOtherParty = table.Column<bool>(type: "bit", nullable: false),
                    HasContractNo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentPatterns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodsProperties",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PropertyType = table.Column<byte>(type: "tinyint", nullable: false),
                    MinimumValue = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    MaximumValue = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    NumberDigit = table.Column<int>(type: "int", nullable: true),
                    MaxLength = table.Column<int>(type: "int", nullable: true),
                    FixLength = table.Column<bool>(type: "bit", nullable: true),
                    Length = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsProperties", x => x.Id);
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
                name: "MeasurementUnits",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MeasurementDimension = table.Column<byte>(type: "tinyint", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductClasses",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PricingMethod = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertiesList",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GoodsPropertyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertiesList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertiesList_GoodsProperties_GoodsPropertyId",
                        column: x => x.GoodsPropertyId,
                        principalSchema: "wh",
                        principalTable: "GoodsProperties",
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
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    PostCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
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

            migrationBuilder.CreateTable(
                name: "StandardChangeUnits",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MainUnitId = table.Column<int>(type: "int", nullable: false),
                    OtherUnitId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardChangeUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StandardChangeUnits_MeasurementUnits_MainUnitId",
                        column: x => x.MainUnitId,
                        principalSchema: "wh",
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StandardChangeUnits_MeasurementUnits_OtherUnitId",
                        column: x => x.OtherUnitId,
                        principalSchema: "wh",
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Goods",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarcodePatternId = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: false),
                    ProductClassId = table.Column<int>(type: "int", nullable: false),
                    GoodsNature = table.Column<byte>(type: "tinyint", nullable: false),
                    GoodsType = table.Column<byte>(type: "tinyint", nullable: false),
                    ReservationLevel = table.Column<byte>(type: "tinyint", nullable: true),
                    ReserveBaseOnControl = table.Column<bool>(type: "bit", nullable: false),
                    SuspendedInIncoming = table.Column<bool>(type: "bit", nullable: false),
                    SuspendedInOutcoming = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goods_BarcodePatterns_BarcodePatternId",
                        column: x => x.BarcodePatternId,
                        principalSchema: "wh",
                        principalTable: "BarcodePatterns",
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

            migrationBuilder.CreateTable(
                name: "Reservations",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpirationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ReservationNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OtherPartyType = table.Column<byte>(type: "tinyint", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    ReservationBasis = table.Column<byte>(type: "tinyint", nullable: false),
                    ReservationType = table.Column<byte>(type: "tinyint", nullable: false),
                    KeepCenterId = table.Column<int>(type: "int", nullable: false),
                    StockRoomId = table.Column<int>(type: "int", nullable: true),
                    Branch = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalSchema: "acc",
                        principalTable: "CostCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservations_KeepCenters_KeepCenterId",
                        column: x => x.KeepCenterId,
                        principalSchema: "wh",
                        principalTable: "KeepCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservations_StockRooms_StockRoomId",
                        column: x => x.StockRoomId,
                        principalSchema: "wh",
                        principalTable: "StockRooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockFlow",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeepCenterId = table.Column<int>(type: "int", nullable: false),
                    DocumentPatternId = table.Column<int>(type: "int", nullable: true),
                    StockRoomId = table.Column<int>(type: "int", nullable: true),
                    OtherKeepCenterId = table.Column<int>(type: "int", nullable: true),
                    OtherStockRoomId = table.Column<int>(type: "int", nullable: true),
                    OtherPartyType = table.Column<byte>(type: "tinyint", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContractNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Branch = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AccountingCodeId = table.Column<int>(type: "int", nullable: true),
                    StockFlowType = table.Column<byte>(type: "tinyint", nullable: false),
                    ReferenceDocumentType = table.Column<byte>(type: "tinyint", nullable: true),
                    RequisitionerId = table.Column<int>(type: "int", nullable: true),
                    StockRoomDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Loadable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockFlow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockFlow_AccountingCodes_AccountingCodeId",
                        column: x => x.AccountingCodeId,
                        principalSchema: "acc",
                        principalTable: "AccountingCodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockFlow_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalSchema: "acc",
                        principalTable: "CostCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockFlow_DocumentPatterns_DocumentPatternId",
                        column: x => x.DocumentPatternId,
                        principalSchema: "wh",
                        principalTable: "DocumentPatterns",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockFlow_KeepCenters_KeepCenterId",
                        column: x => x.KeepCenterId,
                        principalSchema: "wh",
                        principalTable: "KeepCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockFlow_KeepCenters_OtherKeepCenterId",
                        column: x => x.OtherKeepCenterId,
                        principalSchema: "wh",
                        principalTable: "KeepCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockFlow_StockRooms_OtherStockRoomId",
                        column: x => x.OtherStockRoomId,
                        principalSchema: "wh",
                        principalTable: "StockRooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockFlow_StockRooms_StockRoomId",
                        column: x => x.StockRoomId,
                        principalSchema: "wh",
                        principalTable: "StockRooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockFlow_Users_RequisitionerId",
                        column: x => x.RequisitionerId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GoodsPlacements",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    StockRoomId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsPlacements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsPlacements_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodsPlacements_MaterialLocations_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "wh",
                        principalTable: "MaterialLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodsPlacements_StockRooms_StockRoomId",
                        column: x => x.StockRoomId,
                        principalSchema: "wh",
                        principalTable: "StockRooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubstituteProducts",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    SubstituteGoodsId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(6,3)", precision: 6, scale: 3, nullable: false),
                    DuplicateRelation = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubstituteProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubstituteProducts_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubstituteProducts_Goods_SubstituteGoodsId",
                        column: x => x.SubstituteGoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReservationGoods",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float(15)", precision: 15, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationGoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationGoods_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReservationGoods_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalSchema: "wh",
                        principalTable: "Reservations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GoodsFlow",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockFlowId = table.Column<int>(type: "int", nullable: false),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsFlow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsFlow_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodsFlow_StockFlow_StockFlowId",
                        column: x => x.StockFlowId,
                        principalSchema: "wh",
                        principalTable: "StockFlow",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Goods_BarcodePatternId",
                schema: "wh",
                table: "Goods",
                column: "BarcodePatternId");

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

            migrationBuilder.CreateIndex(
                name: "IX_GoodsFlow_GoodsId",
                schema: "wh",
                table: "GoodsFlow",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsFlow_StockFlowId",
                schema: "wh",
                table: "GoodsFlow",
                column: "StockFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsPlacements_GoodsId",
                schema: "wh",
                table: "GoodsPlacements",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsPlacements_LocationId",
                schema: "wh",
                table: "GoodsPlacements",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsPlacements_StockRoomId",
                schema: "wh",
                table: "GoodsPlacements",
                column: "StockRoomId");

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

            migrationBuilder.CreateIndex(
                name: "IX_PropertiesList_GoodsPropertyId",
                schema: "wh",
                table: "PropertiesList",
                column: "GoodsPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationGoods_GoodsId",
                schema: "wh",
                table: "ReservationGoods",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationGoods_ReservationId",
                schema: "wh",
                table: "ReservationGoods",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CostCenterId",
                schema: "wh",
                table: "Reservations",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_KeepCenterId",
                schema: "wh",
                table: "Reservations",
                column: "KeepCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_StockRoomId",
                schema: "wh",
                table: "Reservations",
                column: "StockRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_StandardChangeUnits_MainUnitId",
                schema: "wh",
                table: "StandardChangeUnits",
                column: "MainUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_StandardChangeUnits_OtherUnitId",
                schema: "wh",
                table: "StandardChangeUnits",
                column: "OtherUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_AccountingCodeId",
                schema: "wh",
                table: "StockFlow",
                column: "AccountingCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_CostCenterId",
                schema: "wh",
                table: "StockFlow",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_DocumentPatternId",
                schema: "wh",
                table: "StockFlow",
                column: "DocumentPatternId");

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_KeepCenterId",
                schema: "wh",
                table: "StockFlow",
                column: "KeepCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_OtherKeepCenterId",
                schema: "wh",
                table: "StockFlow",
                column: "OtherKeepCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_OtherStockRoomId",
                schema: "wh",
                table: "StockFlow",
                column: "OtherStockRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_RequisitionerId",
                schema: "wh",
                table: "StockFlow",
                column: "RequisitionerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockFlow_StockRoomId",
                schema: "wh",
                table: "StockFlow",
                column: "StockRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_SubstituteProducts_GoodsId",
                schema: "wh",
                table: "SubstituteProducts",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_SubstituteProducts_SubstituteGoodsId",
                schema: "wh",
                table: "SubstituteProducts",
                column: "SubstituteGoodsId");

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

            migrationBuilder.DropTable(
                name: "GoodsFlow",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "GoodsPlacements",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "PropertiesList",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "ReservationGoods",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "StandardChangeUnits",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "SubstituteProducts",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "StockFlow",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "MaterialLocations",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "GoodsProperties",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Reservations",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Goods",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "DocumentPatterns",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "KeepCenters",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "BarcodePatterns",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "MeasurementUnits",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "ProductClasses",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Branches",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Locations",
                schema: "wh");

            migrationBuilder.DropColumn(
                name: "MemberInManyParts",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.DropColumn(
                name: "MemberOfMiddlePart",
                schema: "wh",
                table: "StockRooms");

            migrationBuilder.RenameColumn(
                name: "KeepCenterId",
                schema: "wh",
                table: "StockRooms",
                newName: "FinancialUnitId");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                schema: "wh",
                table: "StockRooms",
                newName: "BudgetUnitId");

            migrationBuilder.RenameIndex(
                name: "IX_StockRooms_KeepCenterId",
                schema: "wh",
                table: "StockRooms",
                newName: "IX_StockRooms_FinancialUnitId");

            migrationBuilder.RenameIndex(
                name: "IX_StockRooms_BranchId",
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

            migrationBuilder.AddColumn<byte>(
                name: "PricingMethodType",
                schema: "wh",
                table: "StockRooms",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "ScopeId",
                schema: "wh",
                table: "StockRooms",
                type: "int",
                nullable: true);

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
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_StockRooms_ScopeId",
                schema: "wh",
                table: "StockRooms",
                column: "ScopeId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_StockRooms_Scopes_ScopeId",
                schema: "wh",
                table: "StockRooms",
                column: "ScopeId",
                principalSchema: "wh",
                principalTable: "Scopes",
                principalColumn: "Id");

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
