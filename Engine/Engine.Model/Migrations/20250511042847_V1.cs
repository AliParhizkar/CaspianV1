using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Engine.Model.Migrations
{
    /// <inheritdoc />
    public partial class V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cmn");

            migrationBuilder.CreateTable(
                name: "EntityTypes",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubSystem = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Namespace = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ValidationType = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExceptionsData",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubSystemKind = table.Column<byte>(type: "tinyint", nullable: false),
                    SourceCodeFileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LineNumber = table.Column<short>(type: "smallint", nullable: true),
                    RepetitionTimes = table.Column<short>(type: "smallint", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ErrorFileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RegisterDate = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExceptionsData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fonts",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fonts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LookupTypes",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EntityTypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LookupTypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuCategories",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SubSystemKind = table.Column<byte>(type: "tinyint", nullable: false),
                    IconFont = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Ordering = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersianDatesTable",
                schema: "cmn",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    PersianDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    Month = table.Column<byte>(type: "tinyint", nullable: false),
                    Day = table.Column<byte>(type: "tinyint", nullable: false),
                    DayOfPersianWeek = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersianDatesTable", x => x.Date);
                });

            migrationBuilder.CreateTable(
                name: "ReportGroups",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SubSystem = table.Column<byte>(type: "tinyint", nullable: false),
                    NameSpace = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ClassTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MethodName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Disable = table.Column<bool>(type: "bit", nullable: false),
                    Descript = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Descript = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    FormRule = table.Column<bool>(type: "bit", nullable: false),
                    ResultType = table.Column<byte>(type: "tinyint", nullable: false),
                    EnumTypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SystemKind = table.Column<byte>(type: "tinyint", nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsersLogins",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IPAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LoginDate = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false),
                    PageUrl = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LoginProvider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProviderKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersLogins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowGroups",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SubSystemKind = table.Column<byte>(type: "tinyint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    URL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MenuCategoryId = table.Column<int>(type: "int", nullable: true),
                    SubSystemKind = table.Column<byte>(type: "tinyint", nullable: true),
                    Ordering = table.Column<int>(type: "int", nullable: false),
                    ShowonMenu = table.Column<bool>(type: "bit", nullable: false),
                    IsDropped = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menus_MenuCategories_MenuCategoryId",
                        column: x => x.MenuCategoryId,
                        principalSchema: "cmn",
                        principalTable: "MenuCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AggregateReportGroupsParameter",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Allis = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ParentParameterId = table.Column<int>(type: "int", nullable: true),
                    AggregateParameterType = table.Column<byte>(type: "tinyint", nullable: false),
                    AggregateFunctionType = table.Column<byte>(type: "tinyint", nullable: true),
                    ReportGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregateReportGroupsParameter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AggregateReportGroupsParameter_AggregateReportGroupsParameter_ParentParameterId",
                        column: x => x.ParentParameterId,
                        principalSchema: "cmn",
                        principalTable: "AggregateReportGroupsParameter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AggregateReportGroupsParameter_ReportGroups_ReportGroupId",
                        column: x => x.ReportGroupId,
                        principalSchema: "cmn",
                        principalTable: "ReportGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReportGroupsParameters",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportGroupId = table.Column<int>(type: "int", nullable: false),
                    PropertyPath = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsKey = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportGroupsParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportGroupsParameters_ReportGroups_ReportGroupId",
                        column: x => x.ReportGroupId,
                        principalSchema: "cmn",
                        principalTable: "ReportGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PrintFileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FilteringFileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReportType = table.Column<byte>(type: "tinyint", nullable: false),
                    SubReportLevel = table.Column<byte>(type: "tinyint", nullable: true),
                    Descript = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReportGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_ReportGroups_ReportGroupId",
                        column: x => x.ReportGroupId,
                        principalSchema: "cmn",
                        principalTable: "ReportGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DataParameters",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResultParameterId = table.Column<int>(type: "int", nullable: false),
                    ParameterType = table.Column<byte>(type: "tinyint", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RuleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataParameters_Rules_RuleId",
                        column: x => x.RuleId,
                        principalSchema: "cmn",
                        principalTable: "Rules",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenType = table.Column<byte>(type: "tinyint", nullable: false),
                    EnTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FaTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    constValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ConstValueType = table.Column<byte>(type: "tinyint", nullable: true),
                    RuleId = table.Column<int>(type: "int", nullable: true),
                    RuleIdValue = table.Column<int>(type: "int", nullable: true),
                    parameterType = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Rules_RuleId",
                        column: x => x.RuleId,
                        principalSchema: "cmn",
                        principalTable: "Rules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tokens_Rules_RuleIdValue",
                        column: x => x.RuleIdValue,
                        principalSchema: "cmn",
                        principalTable: "Rules",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExceptionDetails",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExceptionDataId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExceptionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExceptionDetails_ExceptionsData_ExceptionDataId",
                        column: x => x.ExceptionDataId,
                        principalSchema: "cmn",
                        principalTable: "ExceptionsData",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExceptionDetails_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersMembership",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersMembership", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersMembership_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "cmn",
                        principalTable: "Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UsersMembership_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataModels",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WorkflowGroupId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataModels_WorkflowGroups_WorkflowGroupId",
                        column: x => x.WorkflowGroupId,
                        principalSchema: "cmn",
                        principalTable: "WorkflowGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MenusAccessibility",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenusAccessibility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenusAccessibility_Menus_MenuId",
                        column: x => x.MenuId,
                        principalSchema: "cmn",
                        principalTable: "Menus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MenusAccessibility_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "cmn",
                        principalTable: "Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MenusAccessibility_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "cmn",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AggregateReportsParameters",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    AggregateReportGroupParameterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregateReportsParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AggregateReportsParameters_AggregateReportGroupsParameter_AggregateReportGroupParameterId",
                        column: x => x.AggregateReportGroupParameterId,
                        principalSchema: "cmn",
                        principalTable: "AggregateReportGroupsParameter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AggregateReportsParameters_Reports_ReportId",
                        column: x => x.ReportId,
                        principalSchema: "cmn",
                        principalTable: "Reports",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReportParams",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    CompositionMethodType = table.Column<byte>(type: "tinyint", nullable: true),
                    RuleId = table.Column<int>(type: "int", nullable: true),
                    ReportGroupParameterId = table.Column<int>(type: "int", nullable: false),
                    ReportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportParams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportParams_ReportGroupsParameters_ReportGroupParameterId",
                        column: x => x.ReportGroupParameterId,
                        principalSchema: "cmn",
                        principalTable: "ReportGroupsParameters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportParams_Reports_ReportId",
                        column: x => x.ReportId,
                        principalSchema: "cmn",
                        principalTable: "Reports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportParams_Rules_RuleId",
                        column: x => x.RuleId,
                        principalSchema: "cmn",
                        principalTable: "Rules",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TabPanels",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReportId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TabPanels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TabPanels_Reports_ReportId",
                        column: x => x.ReportId,
                        principalSchema: "cmn",
                        principalTable: "Reports",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DataModelFields",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataModelId = table.Column<int>(type: "int", nullable: true),
                    EntityFullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FieldType = table.Column<byte>(type: "tinyint", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FieldName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EntityTypeId = table.Column<int>(type: "int", nullable: true),
                    IsDetails = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataModelFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataModelFields_DataModels_DataModelId",
                        column: x => x.DataModelId,
                        principalSchema: "cmn",
                        principalTable: "DataModels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DataModelFields_EntityTypes_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalSchema: "cmn",
                        principalTable: "EntityTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Workflows",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowGroupId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DataModelId = table.Column<int>(type: "int", nullable: false),
                    Descript = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workflows_DataModels_DataModelId",
                        column: x => x.DataModelId,
                        principalSchema: "cmn",
                        principalTable: "DataModels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Workflows_WorkflowGroups_WorkflowGroupId",
                        column: x => x.WorkflowGroupId,
                        principalSchema: "cmn",
                        principalTable: "WorkflowGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowsForms",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WorkflowGroupId = table.Column<int>(type: "int", nullable: false),
                    DataModelId = table.Column<int>(type: "int", nullable: false),
                    ColumnCount = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SourceFileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowsForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowsForms_DataModels_DataModelId",
                        column: x => x.DataModelId,
                        principalSchema: "cmn",
                        principalTable: "DataModels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkflowsForms_WorkflowGroups_WorkflowGroupId",
                        column: x => x.WorkflowGroupId,
                        principalSchema: "cmn",
                        principalTable: "WorkflowGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReportControls",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Left = table.Column<int>(type: "int", nullable: true),
                    Top = table.Column<int>(type: "int", nullable: true),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    FaTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EnTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TabPanelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportControls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportControls_TabPanels_TabPanelId",
                        column: x => x.TabPanelId,
                        principalSchema: "cmn",
                        principalTable: "TabPanels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BlazorControls",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Caption = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomeFieldName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ControlType = table.Column<byte>(type: "tinyint", nullable: false),
                    DataModelFieldId = table.Column<int>(type: "int", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TextExpression = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ConditionExpression = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MultiLine = table.Column<bool>(type: "bit", nullable: false),
                    Height = table.Column<byte>(type: "tinyint", nullable: true),
                    OnChange = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LookupTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlazorControls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlazorControls_DataModelFields_DataModelFieldId",
                        column: x => x.DataModelFieldId,
                        principalSchema: "cmn",
                        principalTable: "DataModelFields",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BlazorControls_LookupTypes_LookupTypeId",
                        column: x => x.LookupTypeId,
                        principalSchema: "cmn",
                        principalTable: "LookupTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DataModelOptions",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataModelOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataModelOptions_DataModelFields_FieldId",
                        column: x => x.FieldId,
                        principalSchema: "cmn",
                        principalTable: "DataModelFields",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActivityType = table.Column<byte>(type: "tinyint", nullable: false),
                    TaskType = table.Column<byte>(type: "tinyint", nullable: true),
                    SourceCodeFileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WorkflowFormId = table.Column<int>(type: "int", nullable: true),
                    Left = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false),
                    Top = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false),
                    GatewayType = table.Column<byte>(type: "tinyint", nullable: true),
                    EventTriggerType = table.Column<byte>(type: "tinyint", nullable: true),
                    ActorType = table.Column<byte>(type: "tinyint", nullable: true),
                    WorkflowId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_WorkflowsForms_WorkflowFormId",
                        column: x => x.WorkflowFormId,
                        principalSchema: "cmn",
                        principalTable: "WorkflowsForms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Activities_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalSchema: "cmn",
                        principalTable: "Workflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HtmlRows",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Span = table.Column<byte>(type: "tinyint", nullable: false),
                    WorkflowFormId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtmlRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtmlRows_WorkflowsForms_WorkflowFormId",
                        column: x => x.WorkflowFormId,
                        principalSchema: "cmn",
                        principalTable: "WorkflowsForms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Connectors",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CheckValidation = table.Column<bool>(type: "bit", nullable: false),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    PortType = table.Column<byte>(type: "tinyint", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CompareType = table.Column<byte>(type: "tinyint", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    ToActivityId = table.Column<int>(type: "int", nullable: false),
                    ToPortType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connectors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Connectors_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "cmn",
                        principalTable: "Activities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Connectors_Activities_ToActivityId",
                        column: x => x.ToActivityId,
                        principalSchema: "cmn",
                        principalTable: "Activities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskOperations",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    ConnectorId = table.Column<int>(type: "int", nullable: false),
                    No = table.Column<int>(type: "int", nullable: false),
                    IsLaste = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskOperations_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "cmn",
                        principalTable: "Activities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskOperations_Connectors_ConnectorId",
                        column: x => x.ConnectorId,
                        principalSchema: "cmn",
                        principalTable: "Connectors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HtmlColumns",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Span = table.Column<byte>(type: "tinyint", nullable: true),
                    Hidden = table.Column<bool>(type: "bit", nullable: false),
                    RowId = table.Column<int>(type: "int", nullable: true),
                    InnerRowId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtmlColumns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtmlColumns_BlazorControls_Id",
                        column: x => x.Id,
                        principalSchema: "cmn",
                        principalTable: "BlazorControls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HtmlColumns_HtmlRows_RowId",
                        column: x => x.RowId,
                        principalSchema: "cmn",
                        principalTable: "HtmlRows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InnerRows",
                schema: "cmn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Span = table.Column<byte>(type: "tinyint", nullable: false),
                    HtmlColumnId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InnerRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InnerRows_HtmlColumns_HtmlColumnId",
                        column: x => x.HtmlColumnId,
                        principalSchema: "cmn",
                        principalTable: "HtmlColumns",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_WorkflowFormId",
                schema: "cmn",
                table: "Activities",
                column: "WorkflowFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_WorkflowId",
                schema: "cmn",
                table: "Activities",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregateReportGroupsParameter_ParentParameterId",
                schema: "cmn",
                table: "AggregateReportGroupsParameter",
                column: "ParentParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregateReportGroupsParameter_ReportGroupId",
                schema: "cmn",
                table: "AggregateReportGroupsParameter",
                column: "ReportGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregateReportsParameters_AggregateReportGroupParameterId",
                schema: "cmn",
                table: "AggregateReportsParameters",
                column: "AggregateReportGroupParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregateReportsParameters_ReportId",
                schema: "cmn",
                table: "AggregateReportsParameters",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_BlazorControls_DataModelFieldId",
                schema: "cmn",
                table: "BlazorControls",
                column: "DataModelFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_BlazorControls_LookupTypeId",
                schema: "cmn",
                table: "BlazorControls",
                column: "LookupTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Connectors_ActivityId",
                schema: "cmn",
                table: "Connectors",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Connectors_ToActivityId",
                schema: "cmn",
                table: "Connectors",
                column: "ToActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_DataModelFields_DataModelId",
                schema: "cmn",
                table: "DataModelFields",
                column: "DataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_DataModelFields_EntityTypeId",
                schema: "cmn",
                table: "DataModelFields",
                column: "EntityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DataModelOptions_FieldId",
                schema: "cmn",
                table: "DataModelOptions",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_DataModels_WorkflowGroupId",
                schema: "cmn",
                table: "DataModels",
                column: "WorkflowGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DataParameters_RuleId",
                schema: "cmn",
                table: "DataParameters",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ExceptionDetails_ExceptionDataId",
                schema: "cmn",
                table: "ExceptionDetails",
                column: "ExceptionDataId");

            migrationBuilder.CreateIndex(
                name: "IX_ExceptionDetails_UserId",
                schema: "cmn",
                table: "ExceptionDetails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HtmlColumns_InnerRowId",
                schema: "cmn",
                table: "HtmlColumns",
                column: "InnerRowId");

            migrationBuilder.CreateIndex(
                name: "IX_HtmlColumns_RowId",
                schema: "cmn",
                table: "HtmlColumns",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_HtmlRows_WorkflowFormId",
                schema: "cmn",
                table: "HtmlRows",
                column: "WorkflowFormId");

            migrationBuilder.CreateIndex(
                name: "IX_InnerRows_HtmlColumnId",
                schema: "cmn",
                table: "InnerRows",
                column: "HtmlColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_MenuCategoryId",
                schema: "cmn",
                table: "Menus",
                column: "MenuCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MenusAccessibility_MenuId",
                schema: "cmn",
                table: "MenusAccessibility",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_MenusAccessibility_RoleId",
                schema: "cmn",
                table: "MenusAccessibility",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MenusAccessibility_UserId",
                schema: "cmn",
                table: "MenusAccessibility",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportControls_TabPanelId",
                schema: "cmn",
                table: "ReportControls",
                column: "TabPanelId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportGroupsParameters_ReportGroupId",
                schema: "cmn",
                table: "ReportGroupsParameters",
                column: "ReportGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportParams_ReportGroupParameterId",
                schema: "cmn",
                table: "ReportParams",
                column: "ReportGroupParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportParams_ReportId",
                schema: "cmn",
                table: "ReportParams",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportParams_RuleId",
                schema: "cmn",
                table: "ReportParams",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportGroupId",
                schema: "cmn",
                table: "Reports",
                column: "ReportGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TabPanels_ReportId",
                schema: "cmn",
                table: "TabPanels",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskOperations_ActivityId",
                schema: "cmn",
                table: "TaskOperations",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskOperations_ConnectorId",
                schema: "cmn",
                table: "TaskOperations",
                column: "ConnectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_RuleId",
                schema: "cmn",
                table: "Tokens",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_RuleIdValue",
                schema: "cmn",
                table: "Tokens",
                column: "RuleIdValue");

            migrationBuilder.CreateIndex(
                name: "IX_UsersMembership_RoleId",
                schema: "cmn",
                table: "UsersMembership",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersMembership_UserId",
                schema: "cmn",
                table: "UsersMembership",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_DataModelId",
                schema: "cmn",
                table: "Workflows",
                column: "DataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_WorkflowGroupId",
                schema: "cmn",
                table: "Workflows",
                column: "WorkflowGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowsForms_DataModelId",
                schema: "cmn",
                table: "WorkflowsForms",
                column: "DataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowsForms_WorkflowGroupId",
                schema: "cmn",
                table: "WorkflowsForms",
                column: "WorkflowGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_HtmlColumns_InnerRows_InnerRowId",
                schema: "cmn",
                table: "HtmlColumns",
                column: "InnerRowId",
                principalSchema: "cmn",
                principalTable: "InnerRows",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HtmlRows_WorkflowsForms_WorkflowFormId",
                schema: "cmn",
                table: "HtmlRows");

            migrationBuilder.DropForeignKey(
                name: "FK_BlazorControls_DataModelFields_DataModelFieldId",
                schema: "cmn",
                table: "BlazorControls");

            migrationBuilder.DropForeignKey(
                name: "FK_BlazorControls_LookupTypes_LookupTypeId",
                schema: "cmn",
                table: "BlazorControls");

            migrationBuilder.DropForeignKey(
                name: "FK_HtmlColumns_BlazorControls_Id",
                schema: "cmn",
                table: "HtmlColumns");

            migrationBuilder.DropForeignKey(
                name: "FK_HtmlColumns_HtmlRows_RowId",
                schema: "cmn",
                table: "HtmlColumns");

            migrationBuilder.DropForeignKey(
                name: "FK_HtmlColumns_InnerRows_InnerRowId",
                schema: "cmn",
                table: "HtmlColumns");

            migrationBuilder.DropTable(
                name: "AggregateReportsParameters",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "DataModelOptions",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "DataParameters",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "ExceptionDetails",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "Fonts",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "MenusAccessibility",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "PersianDatesTable",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "ReportControls",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "ReportParams",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "TaskOperations",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "Tokens",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "UsersLogins",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "UsersMembership",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "AggregateReportGroupsParameter",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "ExceptionsData",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "Menus",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "TabPanels",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "ReportGroupsParameters",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "Connectors",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "Rules",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "MenuCategories",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "Reports",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "Activities",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "ReportGroups",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "Workflows",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "WorkflowsForms",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "DataModelFields",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "DataModels",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "EntityTypes",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "WorkflowGroups",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "LookupTypes",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "BlazorControls",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "HtmlRows",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "InnerRows",
                schema: "cmn");

            migrationBuilder.DropTable(
                name: "HtmlColumns",
                schema: "cmn");
        }
    }
}
