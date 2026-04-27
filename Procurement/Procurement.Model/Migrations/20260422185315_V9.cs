using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Procurement.Model.Migrations
{
    /// <inheritdoc />
    public partial class V9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "SupplyingUnitsMembership",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "SupplyingUnits",
                schema: "pcm");
        }
    }
}
