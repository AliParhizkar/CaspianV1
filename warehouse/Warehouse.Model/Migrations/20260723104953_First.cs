using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "wh");

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
                name: "InventoryControlAgents",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsPropertiesId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    CombinedInventory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryControlAgents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryControlAgents_GoodsProperties_GoodsPropertiesId",
                        column: x => x.GoodsPropertiesId,
                        principalSchema: "wh",
                        principalTable: "GoodsProperties",
                        principalColumn: "Id");
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
                    SuspendedInOutcoming = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "StockRooms",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    KeepCenterId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    MemberOfMiddlePart = table.Column<bool>(type: "bit", nullable: false),
                    MemberInManyParts = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockRooms_Branches_BranchId",
                        column: x => x.BranchId,
                        principalSchema: "wh",
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockRooms_KeepCenters_KeepCenterId",
                        column: x => x.KeepCenterId,
                        principalSchema: "wh",
                        principalTable: "KeepCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockRooms_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GoodsOrderings",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    KeepCenterId = table.Column<int>(type: "int", nullable: false),
                    MinimumInventory = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    MaximumInventory = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    OrderingPoint = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    EconomicOrder = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ConsumeUltimate = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsOrderings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsOrderings_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodsOrderings_KeepCenters_KeepCenterId",
                        column: x => x.KeepCenterId,
                        principalSchema: "wh",
                        principalTable: "KeepCenters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PropertiesOfGoods",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsPropertiesId = table.Column<int>(type: "int", nullable: false),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    BooleanField = table.Column<bool>(type: "bit", nullable: true),
                    StringField = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NumericField = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    DateField = table.Column<DateOnly>(type: "date", nullable: true),
                    PropertyListIdField = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertiesOfGoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertiesOfGoods_GoodsProperties_GoodsPropertiesId",
                        column: x => x.GoodsPropertiesId,
                        principalSchema: "wh",
                        principalTable: "GoodsProperties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropertiesOfGoods_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropertiesOfGoods_PropertiesList_PropertyListIdField",
                        column: x => x.PropertyListIdField,
                        principalSchema: "wh",
                        principalTable: "PropertiesList",
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
                    table.ForeignKey(
                        name: "FK_Reservations_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "pcm",
                        principalTable: "Suppliers",
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
                        name: "FK_StockFlow_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "pcm",
                        principalTable: "Suppliers",
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
                name: "IX_GoodsOrderings_GoodsId",
                schema: "wh",
                table: "GoodsOrderings",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsOrderings_KeepCenterId",
                schema: "wh",
                table: "GoodsOrderings",
                column: "KeepCenterId");

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
                name: "IX_InventoryControlAgents_GoodsPropertiesId",
                schema: "wh",
                table: "InventoryControlAgents",
                column: "GoodsPropertiesId");

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
                name: "IX_PropertiesOfGoods_GoodsId",
                schema: "wh",
                table: "PropertiesOfGoods",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertiesOfGoods_GoodsPropertiesId",
                schema: "wh",
                table: "PropertiesOfGoods",
                column: "GoodsPropertiesId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertiesOfGoods_PropertyListIdField",
                schema: "wh",
                table: "PropertiesOfGoods",
                column: "PropertyListIdField");

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
                name: "IX_Reservations_SupplierId",
                schema: "wh",
                table: "Reservations",
                column: "SupplierId");

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
                name: "IX_StockFlow_SupplierId",
                schema: "wh",
                table: "StockFlow",
                column: "SupplierId");

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
                name: "IX_StockRooms_ManagerId",
                schema: "wh",
                table: "StockRooms",
                column: "ManagerId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodsFlow",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "GoodsOrderings",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "GoodsPlacements",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "InventoryControlAgents",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "PropertiesOfGoods",
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
                name: "PropertiesList",
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
                name: "GoodsProperties",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "StockRooms",
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
                name: "KeepCenters",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Branches",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "Locations",
                schema: "wh");
        }
    }
}
