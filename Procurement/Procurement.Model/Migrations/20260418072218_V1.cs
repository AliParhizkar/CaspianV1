using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Procurement.Model.Migrations
{
    /// <inheritdoc />
    public partial class V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pcm");

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

            migrationBuilder.Sql("ALTER SCHEMA cmn TRANSFER whr.Suppliers");

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
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingSpecialist_UserId",
                schema: "pcm",
                table: "PurchasingSpecialist",
                column: "UserId");

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
                name: "IX_Suppliers_UserId",
                schema: "pcm",
                table: "Suppliers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchasingSpecialist",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "SupplierEvaluationDetails",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "EvaluationIndicators",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "SupplierEvaluations",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "Suppliers",
                schema: "pcm");
        }
    }
}
