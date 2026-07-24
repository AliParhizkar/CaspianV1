using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Procurement.Model.Migrations
{
    /// <inheritdoc />
    public partial class Second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EvaluationIndicators",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IndicatorType = table.Column<byte>(type: "tinyint", nullable: false),
                    EffectingType = table.Column<byte>(type: "tinyint", nullable: false),
                    Minimum = table.Column<int>(type: "int", nullable: false),
                    Maximum = table.Column<int>(type: "int", nullable: false),
                    Factor = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationIndicators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Policies",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PolicyKind = table.Column<byte>(type: "tinyint", nullable: true),
                    EffectingLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    CalculatingMethod = table.Column<byte>(type: "tinyint", nullable: false),
                    ParticipatoryApproach = table.Column<byte>(type: "tinyint", nullable: false),
                    Definiteness = table.Column<byte>(type: "tinyint", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DocumentKind = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyParameters",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Property = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyParameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcurementItemGroups",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ParentGroupId = table.Column<int>(type: "int", nullable: true),
                    CodingLevels = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcurementItemGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcurementItemGroups_ProcurementItemGroups_ParentGroupId",
                        column: x => x.ParentGroupId,
                        principalSchema: "pcm",
                        principalTable: "ProcurementItemGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequests",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ProcurementItemType = table.Column<byte>(type: "tinyint", nullable: false),
                    KeepCenterId = table.Column<int>(type: "int", nullable: false),
                    RequesterId = table.Column<int>(type: "int", nullable: true),
                    OtherPartyType = table.Column<byte>(type: "tinyint", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PurchaseRequestType = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalSchema: "acc",
                        principalTable: "CostCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_KeepCenters_KeepCenterId",
                        column: x => x.KeepCenterId,
                        principalSchema: "wh",
                        principalTable: "KeepCenters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "pcm",
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_Users_RequesterId",
                        column: x => x.RequesterId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseTypes",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PurchaseProcessType = table.Column<byte>(type: "tinyint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchasingSpecialist",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasingSpecialist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchasingSpecialist_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sellers",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonType = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EconomicCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Citizenship = table.Column<byte>(type: "tinyint", nullable: false),
                    IdCard = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PostCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AreaCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tell = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tell2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sellers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sellers_Locations_CityId",
                        column: x => x.CityId,
                        principalSchema: "wh",
                        principalTable: "Locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sellers_Locations_ProvinceId",
                        column: x => x.ProvinceId,
                        principalSchema: "wh",
                        principalTable: "Locations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Services",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_MeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalSchema: "wh",
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SupplierEvaluations",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    EvaluationBaseType = table.Column<byte>(type: "tinyint", nullable: true),
                    ReceiptId = table.Column<int>(type: "int", nullable: true),
                    SupplierRank = table.Column<byte>(type: "tinyint", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierEvaluations_StockFlow_ReceiptId",
                        column: x => x.ReceiptId,
                        principalSchema: "wh",
                        principalTable: "StockFlow",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplierEvaluations_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "pcm",
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SupplierGroups",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ParentGroupId = table.Column<int>(type: "int", nullable: true),
                    CodingLevels = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierGroups_SupplierGroups_ParentGroupId",
                        column: x => x.ParentGroupId,
                        principalSchema: "pcm",
                        principalTable: "SupplierGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SupplyingUnits",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodingLevels = table.Column<byte>(type: "tinyint", nullable: false),
                    ParentUnitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyingUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplyingUnits_Branches_BranchId",
                        column: x => x.BranchId,
                        principalSchema: "wh",
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplyingUnits_SupplyingUnits_ParentUnitId",
                        column: x => x.ParentUnitId,
                        principalSchema: "pcm",
                        principalTable: "SupplyingUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplyingUnits_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UrgentPurchases",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrgentPurchases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyParameterConditions",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyId = table.Column<int>(type: "int", nullable: false),
                    ParameterId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyParameterConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyParameterConditions_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalSchema: "pcm",
                        principalTable: "Policies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PolicyParameterConditions_PolicyParameters_ParameterId",
                        column: x => x.ParameterId,
                        principalSchema: "pcm",
                        principalTable: "PolicyParameters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProcurementItem",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcurementItemType = table.Column<byte>(type: "tinyint", nullable: false),
                    GoodsId = table.Column<int>(type: "int", nullable: true),
                    ServiceId = table.Column<int>(type: "int", nullable: true),
                    ISActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcurementItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcurementItem_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProcurementItem_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "pcm",
                        principalTable: "Services",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SupplierEvaluationDetails",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierEvaluationId = table.Column<int>(type: "int", nullable: false),
                    EvaluationIndicatorId = table.Column<int>(type: "int", nullable: false),
                    Privilege = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierEvaluationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierEvaluationDetails_EvaluationIndicators_EvaluationIndicatorId",
                        column: x => x.EvaluationIndicatorId,
                        principalSchema: "pcm",
                        principalTable: "EvaluationIndicators",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplierEvaluationDetails_SupplierEvaluations_SupplierEvaluationId",
                        column: x => x.SupplierEvaluationId,
                        principalSchema: "pcm",
                        principalTable: "SupplierEvaluations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SupplierGroupMemberships",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    SupplierGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierGroupMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierGroupMemberships_SupplierGroups_SupplierGroupId",
                        column: x => x.SupplierGroupId,
                        principalSchema: "pcm",
                        principalTable: "SupplierGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplierGroupMemberships_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "pcm",
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PriceInquiries",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InquiryNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InquiryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    InquiryDeadline = table.Column<DateOnly>(type: "date", nullable: true),
                    SupplyingUnitId = table.Column<int>(type: "int", nullable: false),
                    MultiplePreInvoice = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceInquiries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceInquiries_SupplyingUnits_SupplyingUnitId",
                        column: x => x.SupplyingUnitId,
                        principalSchema: "pcm",
                        principalTable: "SupplyingUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SupplyingUnitsMembership",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplyingUnitId = table.Column<int>(type: "int", nullable: false),
                    PurchasingSpecialistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyingUnitsMembership", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplyingUnitsMembership_PurchasingSpecialist_PurchasingSpecialistId",
                        column: x => x.PurchasingSpecialistId,
                        principalSchema: "pcm",
                        principalTable: "PurchasingSpecialist",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplyingUnitsMembership_SupplyingUnits_SupplyingUnitId",
                        column: x => x.SupplyingUnitId,
                        principalSchema: "pcm",
                        principalTable: "SupplyingUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProcurementItemsGrouping",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcurementItemId = table.Column<int>(type: "int", nullable: false),
                    ProcurementItemGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcurementItemsGrouping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcurementItemsGrouping_ProcurementItemGroups_ProcurementItemGroupId",
                        column: x => x.ProcurementItemGroupId,
                        principalSchema: "pcm",
                        principalTable: "ProcurementItemGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProcurementItemsGrouping_ProcurementItem_ProcurementItemId",
                        column: x => x.ProcurementItemId,
                        principalSchema: "pcm",
                        principalTable: "ProcurementItem",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequestsGoods",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseRequestId = table.Column<int>(type: "int", nullable: false),
                    ReferenceDocumentType = table.Column<byte>(type: "tinyint", nullable: false),
                    GoodsFlowId = table.Column<int>(type: "int", nullable: true),
                    ProcurementItemId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    NeedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    UrgentPurchaseId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SupplyingUnitId = table.Column<int>(type: "int", nullable: true),
                    PurchaseTypeId = table.Column<int>(type: "int", nullable: true),
                    InquiryDeadline = table.Column<DateOnly>(type: "date", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    PurchasingSpecialistId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequestsGoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestsGoods_GoodsFlow_GoodsFlowId",
                        column: x => x.GoodsFlowId,
                        principalSchema: "wh",
                        principalTable: "GoodsFlow",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequestsGoods_ProcurementItem_ProcurementItemId",
                        column: x => x.ProcurementItemId,
                        principalSchema: "pcm",
                        principalTable: "ProcurementItem",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequestsGoods_PurchaseRequests_PurchaseRequestId",
                        column: x => x.PurchaseRequestId,
                        principalSchema: "pcm",
                        principalTable: "PurchaseRequests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequestsGoods_PurchaseTypes_PurchaseTypeId",
                        column: x => x.PurchaseTypeId,
                        principalSchema: "pcm",
                        principalTable: "PurchaseTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequestsGoods_PurchasingSpecialist_PurchasingSpecialistId",
                        column: x => x.PurchasingSpecialistId,
                        principalSchema: "pcm",
                        principalTable: "PurchasingSpecialist",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequestsGoods_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "pcm",
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequestsGoods_SupplyingUnits_SupplyingUnitId",
                        column: x => x.SupplyingUnitId,
                        principalSchema: "pcm",
                        principalTable: "SupplyingUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequestsGoods_UrgentPurchases_UrgentPurchaseId",
                        column: x => x.UrgentPurchaseId,
                        principalSchema: "pcm",
                        principalTable: "UrgentPurchases",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SupplyingScopes",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierGroupId = table.Column<int>(type: "int", nullable: false),
                    ProcurementItemGroupId = table.Column<int>(type: "int", nullable: true),
                    ProcurementItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyingScopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplyingScopes_ProcurementItemGroups_ProcurementItemGroupId",
                        column: x => x.ProcurementItemGroupId,
                        principalSchema: "pcm",
                        principalTable: "ProcurementItemGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplyingScopes_ProcurementItem_ProcurementItemId",
                        column: x => x.ProcurementItemId,
                        principalSchema: "pcm",
                        principalTable: "ProcurementItem",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplyingScopes_SupplierGroups_SupplierGroupId",
                        column: x => x.SupplierGroupId,
                        principalSchema: "pcm",
                        principalTable: "SupplierGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PriceInquiriesGoods",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    PriceInquiryId = table.Column<int>(type: "int", nullable: false),
                    ReferenceDocumentType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceInquiriesGoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceInquiriesGoods_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalSchema: "wh",
                        principalTable: "Goods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PriceInquiriesGoods_PriceInquiries_PriceInquiryId",
                        column: x => x.PriceInquiryId,
                        principalSchema: "pcm",
                        principalTable: "PriceInquiries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RequestGoodsSuppliers",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseRequestGoodsId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestGoodsSuppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestGoodsSuppliers_PurchaseRequestsGoods_PurchaseRequestGoodsId",
                        column: x => x.PurchaseRequestGoodsId,
                        principalSchema: "pcm",
                        principalTable: "PurchaseRequestsGoods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequestGoodsSuppliers_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "pcm",
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PolicyParameterConditions_ParameterId",
                schema: "pcm",
                table: "PolicyParameterConditions",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyParameterConditions_PolicyId",
                schema: "pcm",
                table: "PolicyParameterConditions",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceInquiries_SupplyingUnitId",
                schema: "pcm",
                table: "PriceInquiries",
                column: "SupplyingUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceInquiriesGoods_GoodsId",
                schema: "pcm",
                table: "PriceInquiriesGoods",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceInquiriesGoods_PriceInquiryId",
                schema: "pcm",
                table: "PriceInquiriesGoods",
                column: "PriceInquiryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementItem_GoodsId",
                schema: "pcm",
                table: "ProcurementItem",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementItem_ServiceId",
                schema: "pcm",
                table: "ProcurementItem",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementItemGroups_ParentGroupId",
                schema: "pcm",
                table: "ProcurementItemGroups",
                column: "ParentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementItemsGrouping_ProcurementItemGroupId",
                schema: "pcm",
                table: "ProcurementItemsGrouping",
                column: "ProcurementItemGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementItemsGrouping_ProcurementItemId",
                schema: "pcm",
                table: "ProcurementItemsGrouping",
                column: "ProcurementItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_CostCenterId",
                schema: "pcm",
                table: "PurchaseRequests",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_KeepCenterId",
                schema: "pcm",
                table: "PurchaseRequests",
                column: "KeepCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_RequesterId",
                schema: "pcm",
                table: "PurchaseRequests",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_SupplierId",
                schema: "pcm",
                table: "PurchaseRequests",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_GoodsFlowId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "GoodsFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_ProcurementItemId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "ProcurementItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_PurchaseRequestId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "PurchaseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_PurchaseTypeId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "PurchaseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_PurchasingSpecialistId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "PurchasingSpecialistId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_SupplierId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_SupplyingUnitId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "SupplyingUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestsGoods_UrgentPurchaseId",
                schema: "pcm",
                table: "PurchaseRequestsGoods",
                column: "UrgentPurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingSpecialist_UserId",
                schema: "pcm",
                table: "PurchasingSpecialist",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestGoodsSuppliers_PurchaseRequestGoodsId",
                schema: "pcm",
                table: "RequestGoodsSuppliers",
                column: "PurchaseRequestGoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestGoodsSuppliers_SupplierId",
                schema: "pcm",
                table: "RequestGoodsSuppliers",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_CityId",
                schema: "pcm",
                table: "Sellers",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_ProvinceId",
                schema: "pcm",
                table: "Sellers",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_MeasurementUnitId",
                schema: "pcm",
                table: "Services",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierEvaluationDetails_EvaluationIndicatorId",
                schema: "pcm",
                table: "SupplierEvaluationDetails",
                column: "EvaluationIndicatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierEvaluationDetails_SupplierEvaluationId",
                schema: "pcm",
                table: "SupplierEvaluationDetails",
                column: "SupplierEvaluationId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierEvaluations_ReceiptId",
                schema: "pcm",
                table: "SupplierEvaluations",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierEvaluations_SupplierId",
                schema: "pcm",
                table: "SupplierEvaluations",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierGroupMemberships_SupplierGroupId",
                schema: "pcm",
                table: "SupplierGroupMemberships",
                column: "SupplierGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierGroupMemberships_SupplierId",
                schema: "pcm",
                table: "SupplierGroupMemberships",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierGroups_ParentGroupId",
                schema: "pcm",
                table: "SupplierGroups",
                column: "ParentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyingScopes_ProcurementItemGroupId",
                schema: "pcm",
                table: "SupplyingScopes",
                column: "ProcurementItemGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyingScopes_ProcurementItemId",
                schema: "pcm",
                table: "SupplyingScopes",
                column: "ProcurementItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyingScopes_SupplierGroupId",
                schema: "pcm",
                table: "SupplyingScopes",
                column: "SupplierGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyingUnits_BranchId",
                schema: "pcm",
                table: "SupplyingUnits",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyingUnits_ManagerId",
                schema: "pcm",
                table: "SupplyingUnits",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyingUnits_ParentUnitId",
                schema: "pcm",
                table: "SupplyingUnits",
                column: "ParentUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyingUnitsMembership_PurchasingSpecialistId",
                schema: "pcm",
                table: "SupplyingUnitsMembership",
                column: "PurchasingSpecialistId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyingUnitsMembership_SupplyingUnitId",
                schema: "pcm",
                table: "SupplyingUnitsMembership",
                column: "SupplyingUnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PolicyParameterConditions",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "PriceInquiriesGoods",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "ProcurementItemsGrouping",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "RequestGoodsSuppliers",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "Sellers",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "SupplierEvaluationDetails",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "SupplierGroupMemberships",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "SupplyingScopes",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "SupplyingUnitsMembership",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "Policies",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "PolicyParameters",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "PriceInquiries",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "PurchaseRequestsGoods",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "EvaluationIndicators",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "SupplierEvaluations",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "ProcurementItemGroups",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "SupplierGroups",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "ProcurementItem",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "PurchaseRequests",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "PurchaseTypes",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "PurchasingSpecialist",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "SupplyingUnits",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "UrgentPurchases",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "Services",
                schema: "pcm");
        }
    }
}
