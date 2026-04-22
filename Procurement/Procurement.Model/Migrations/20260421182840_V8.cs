using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Procurement.Model.Migrations
{
    /// <inheritdoc />
    public partial class V8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplyingScopes",
                schema: "pcm");
        }
    }
}
