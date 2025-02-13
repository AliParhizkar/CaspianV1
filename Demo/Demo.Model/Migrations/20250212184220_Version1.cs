using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Model.Migrations
{
    /// <inheritdoc />
    public partial class Version1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "HR");

            migrationBuilder.EnsureSchema(
                name: "demo");

            migrationBuilder.EnsureSchema(
                name: "hr");

            migrationBuilder.EnsureSchema(
                name: "Demo");

            migrationBuilder.CreateTable(
                name: "AddressTypes",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries ",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActiveType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries ", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "couriers",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_couriers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerType = table.Column<byte>(type: "tinyint", nullable: false),
                    FName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Gender = table.Column<byte>(type: "tinyint", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomerNumber = table.Column<int>(type: "int", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomersGroups",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActiveType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomersGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MainUnits",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActiveType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meetings",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RecordingVideoIs = table.Column<bool>(type: "bit", nullable: false),
                    RecordingAudioIs = table.Column<bool>(type: "bit", nullable: false),
                    ActiveIs = table.Column<bool>(type: "bit", nullable: false),
                    LearningScope = table.Column<byte>(type: "tinyint", nullable: false),
                    ServerType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meetings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meetings_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrganUnits",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentOrganId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActiveType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganUnits_OrganUnits_ParentOrganId",
                        column: x => x.ParentOrganId,
                        principalSchema: "demo",
                        principalTable: "OrganUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Ordering = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActiveType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scopes",
                schema: "HR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EnTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SimpleData",
                schema: "Demo",
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
                name: "Test",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActiveType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActiveType = table.Column<byte>(type: "tinyint", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Provinces_Countries _CountryId",
                        column: x => x.CountryId,
                        principalSchema: "demo",
                        principalTable: "Countries ",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomersAddresses",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddressTypeId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomersAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomersAddresses_AddressTypes_AddressTypeId",
                        column: x => x.AddressTypeId,
                        principalSchema: "demo",
                        principalTable: "AddressTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomersAddresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "demo",
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    OrderNo = table.Column<int>(type: "int", nullable: true),
                    CourierId = table.Column<int>(type: "int", nullable: true),
                    OrderType = table.Column<byte>(type: "tinyint", nullable: false),
                    OrderStatus = table.Column<byte>(type: "tinyint", nullable: true),
                    TotalAmount = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "demo",
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_couriers_CourierId",
                        column: x => x.CourierId,
                        principalSchema: "demo",
                        principalTable: "couriers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerGroupsMembership",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerGroupId = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerGroupsMembership", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerGroupsMembership_CustomersGroups_CustomerGroupId",
                        column: x => x.CustomerGroupId,
                        principalSchema: "demo",
                        principalTable: "CustomersGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerGroupsMembership_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "demo",
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Subunits",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MainUnitId = table.Column<int>(type: "int", nullable: false),
                    Factor = table.Column<int>(type: "int", nullable: false),
                    ActiveType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subunits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subunits_MainUnits_MainUnitId",
                        column: x => x.MainUnitId,
                        principalSchema: "demo",
                        principalTable: "MainUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProductCategoryId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: true),
                    TakeoutPrice = table.Column<int>(type: "int", nullable: false),
                    Meal = table.Column<byte>(type: "tinyint", nullable: false),
                    OutofStock = table.Column<bool>(type: "bit", nullable: false),
                    Discountable = table.Column<bool>(type: "bit", nullable: false),
                    ActiveType = table.Column<byte>(type: "tinyint", nullable: false),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalSchema: "demo",
                        principalTable: "ProductCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Evaluations",
                schema: "HR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScopeId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EnTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false),
                    EvaluationTurn = table.Column<byte>(type: "tinyint", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EvalDateFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    EvalDateTo = table.Column<DateOnly>(type: "date", nullable: true),
                    ReviewDateFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    ReviewDateTo = table.Column<DateOnly>(type: "date", nullable: true),
                    NotificationDateFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    NotificationDateTo = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evaluations_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalSchema: "HR",
                        principalTable: "Scopes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                schema: "HR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScopeId = table.Column<int>(type: "int", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    EmploymentTypeId = table.Column<int>(type: "int", nullable: true),
                    EmployeeTypeId = table.Column<int>(type: "int", nullable: true),
                    FName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Gender = table.Column<byte>(type: "tinyint", nullable: false),
                    IdCard = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FileNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmploymentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_SimpleData_CostCenterId",
                        column: x => x.CostCenterId,
                        principalSchema: "Demo",
                        principalTable: "SimpleData",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_SimpleData_EmployeeTypeId",
                        column: x => x.EmployeeTypeId,
                        principalSchema: "Demo",
                        principalTable: "SimpleData",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_SimpleData_EmploymentTypeId",
                        column: x => x.EmploymentTypeId,
                        principalSchema: "Demo",
                        principalTable: "SimpleData",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_SimpleData_ScopeId",
                        column: x => x.ScopeId,
                        principalSchema: "Demo",
                        principalTable: "SimpleData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubReligions",
                schema: "HR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReligionId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubReligions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubReligions_SimpleData_ReligionId",
                        column: x => x.ReligionId,
                        principalSchema: "Demo",
                        principalTable: "SimpleData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WarehouseReceipt",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseReceipt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseReceipt_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalSchema: "demo",
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: true),
                    ActiveType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Countries _CountryId",
                        column: x => x.CountryId,
                        principalSchema: "demo",
                        principalTable: "Countries ",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cities_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalSchema: "demo",
                        principalTable: "Provinces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MainUnitId = table.Column<int>(type: "int", nullable: false),
                    SubunitId = table.Column<int>(type: "int", nullable: true),
                    ActiveType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materials_MainUnits_MainUnitId",
                        column: x => x.MainUnitId,
                        principalSchema: "demo",
                        principalTable: "MainUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Materials_Subunits_SubunitId",
                        column: x => x.SubunitId,
                        principalSchema: "demo",
                        principalTable: "Subunits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Descript = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Result = table.Column<int>(type: "int", nullable: false, computedColumnSql: "[Price] * [Quantity]")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "demo",
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderDetail_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "demo",
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductsDescription",
                schema: "Demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsDescription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductsDescription_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "demo",
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                schema: "HR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    AddressName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Employees_Id",
                        column: x => x.Id,
                        principalSchema: "HR",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseStudies",
                schema: "HR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseStudies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseStudies_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "HR",
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Families",
                schema: "HR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    WifeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MariageDate = table.Column<DateOnly>(type: "date", nullable: true),
                    WifeJobId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Families", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Families_Employees_Id",
                        column: x => x.Id,
                        principalSchema: "HR",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Families_SimpleData_WifeJobId",
                        column: x => x.WifeJobId,
                        principalSchema: "Demo",
                        principalTable: "SimpleData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReligionAndSubReligion",
                schema: "HR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ReligionId = table.Column<int>(type: "int", nullable: true),
                    SubReligionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReligionAndSubReligion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReligionAndSubReligion_Employees_Id",
                        column: x => x.Id,
                        principalSchema: "HR",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReligionAndSubReligion_SimpleData_ReligionId",
                        column: x => x.ReligionId,
                        principalSchema: "Demo",
                        principalTable: "SimpleData",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReligionAndSubReligion_SubReligions_SubReligionId",
                        column: x => x.SubReligionId,
                        principalSchema: "HR",
                        principalTable: "SubReligions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IdentificationDetails",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    FatherName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IdentificationNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IdentificationSerial = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    BirthCountryId = table.Column<int>(type: "int", nullable: true),
                    BirthProvinceId = table.Column<int>(type: "int", nullable: true),
                    BirthCityId = table.Column<int>(type: "int", nullable: true),
                    RegCountryId = table.Column<int>(type: "int", nullable: true),
                    RegProvinceId = table.Column<int>(type: "int", nullable: true),
                    RegCityId = table.Column<int>(type: "int", nullable: true),
                    Village = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentificationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentificationDetails_Cities_BirthCityId",
                        column: x => x.BirthCityId,
                        principalSchema: "demo",
                        principalTable: "Cities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdentificationDetails_Cities_RegCityId",
                        column: x => x.RegCityId,
                        principalSchema: "demo",
                        principalTable: "Cities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdentificationDetails_Countries _BirthCountryId",
                        column: x => x.BirthCountryId,
                        principalSchema: "demo",
                        principalTable: "Countries ",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdentificationDetails_Countries _RegCountryId",
                        column: x => x.RegCountryId,
                        principalSchema: "demo",
                        principalTable: "Countries ",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdentificationDetails_Employees_Id",
                        column: x => x.Id,
                        principalSchema: "HR",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentificationDetails_Provinces_BirthProvinceId",
                        column: x => x.BirthProvinceId,
                        principalSchema: "demo",
                        principalTable: "Provinces",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdentificationDetails_Provinces_RegProvinceId",
                        column: x => x.RegProvinceId,
                        principalSchema: "demo",
                        principalTable: "Provinces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReceiptDetails",
                schema: "demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptId = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    QuantityMain = table.Column<int>(type: "int", nullable: false),
                    QuantitySub = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptDetails_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalSchema: "demo",
                        principalTable: "Materials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReceiptDetails_WarehouseReceipt_ReceiptId",
                        column: x => x.ReceiptId,
                        principalSchema: "demo",
                        principalTable: "WarehouseReceipt",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                schema: "demo",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ProvinceId",
                schema: "demo",
                table: "Cities",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudies_EmployeeId",
                schema: "HR",
                table: "CourseStudies",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerGroupsMembership_CustomerGroupId",
                schema: "demo",
                table: "CustomerGroupsMembership",
                column: "CustomerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerGroupsMembership_CustomerId",
                schema: "demo",
                table: "CustomerGroupsMembership",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomersAddresses_AddressTypeId",
                schema: "demo",
                table: "CustomersAddresses",
                column: "AddressTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomersAddresses_CustomerId",
                schema: "demo",
                table: "CustomersAddresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CostCenterId",
                schema: "HR",
                table: "Employees",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeTypeId",
                schema: "HR",
                table: "Employees",
                column: "EmployeeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmploymentTypeId",
                schema: "HR",
                table: "Employees",
                column: "EmploymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ScopeId",
                schema: "HR",
                table: "Employees",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_ScopeId",
                schema: "HR",
                table: "Evaluations",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_Families_WifeJobId",
                schema: "HR",
                table: "Families",
                column: "WifeJobId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentificationDetails_BirthCityId",
                schema: "hr",
                table: "IdentificationDetails",
                column: "BirthCityId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentificationDetails_BirthCountryId",
                schema: "hr",
                table: "IdentificationDetails",
                column: "BirthCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentificationDetails_BirthProvinceId",
                schema: "hr",
                table: "IdentificationDetails",
                column: "BirthProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentificationDetails_RegCityId",
                schema: "hr",
                table: "IdentificationDetails",
                column: "RegCityId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentificationDetails_RegCountryId",
                schema: "hr",
                table: "IdentificationDetails",
                column: "RegCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentificationDetails_RegProvinceId",
                schema: "hr",
                table: "IdentificationDetails",
                column: "RegProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_MainUnitId",
                schema: "demo",
                table: "Materials",
                column: "MainUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_SubunitId",
                schema: "demo",
                table: "Materials",
                column: "SubunitId");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_UserId",
                schema: "demo",
                table: "Meetings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderId",
                schema: "demo",
                table: "OrderDetail",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ProductId",
                schema: "demo",
                table: "OrderDetail",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CourierId",
                schema: "demo",
                table: "Orders",
                column: "CourierId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                schema: "demo",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganUnits_ParentOrganId",
                schema: "demo",
                table: "OrganUnits",
                column: "ParentOrganId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductCategoryId",
                schema: "demo",
                table: "Products",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsDescription_ProductId",
                schema: "Demo",
                table: "ProductsDescription",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_CountryId",
                schema: "demo",
                table: "Provinces",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDetails_MaterialId",
                schema: "demo",
                table: "ReceiptDetails",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDetails_ReceiptId",
                schema: "demo",
                table: "ReceiptDetails",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ReligionAndSubReligion_ReligionId",
                schema: "HR",
                table: "ReligionAndSubReligion",
                column: "ReligionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReligionAndSubReligion_SubReligionId",
                schema: "HR",
                table: "ReligionAndSubReligion",
                column: "SubReligionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubReligions_ReligionId",
                schema: "HR",
                table: "SubReligions",
                column: "ReligionId");

            migrationBuilder.CreateIndex(
                name: "IX_Subunits_MainUnitId",
                schema: "demo",
                table: "Subunits",
                column: "MainUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseReceipt_WarehouseId",
                schema: "demo",
                table: "WarehouseReceipt",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses",
                schema: "HR");

            migrationBuilder.DropTable(
                name: "CourseStudies",
                schema: "HR");

            migrationBuilder.DropTable(
                name: "CustomerGroupsMembership",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "CustomersAddresses",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "Evaluations",
                schema: "HR");

            migrationBuilder.DropTable(
                name: "Families",
                schema: "HR");

            migrationBuilder.DropTable(
                name: "IdentificationDetails",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "Meetings",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "OrderDetail",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "OrganUnits",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "ProductsDescription",
                schema: "Demo");

            migrationBuilder.DropTable(
                name: "ReceiptDetails",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "ReligionAndSubReligion",
                schema: "HR");

            migrationBuilder.DropTable(
                name: "Test",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "CustomersGroups",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "AddressTypes",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "Scopes",
                schema: "HR");

            migrationBuilder.DropTable(
                name: "Cities",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "Materials",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "WarehouseReceipt",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "Employees",
                schema: "HR");

            migrationBuilder.DropTable(
                name: "SubReligions",
                schema: "HR");

            migrationBuilder.DropTable(
                name: "Provinces",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "Customers",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "couriers",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "ProductCategories",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "Subunits",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "Warehouses",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "SimpleData",
                schema: "Demo");

            migrationBuilder.DropTable(
                name: "Countries ",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "MainUnits",
                schema: "demo");
        }
    }
}
