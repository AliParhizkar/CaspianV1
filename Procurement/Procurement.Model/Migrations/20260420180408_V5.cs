using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Procurement.Model.Migrations
{
    /// <inheritdoc />
    public partial class V5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcurementItemGroupsMembership",
                schema: "pcm");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcurementItemsGrouping",
                schema: "pcm");

            migrationBuilder.CreateTable(
                name: "ProcurementItemGroupsMembership",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcurementItemGroupId = table.Column<int>(type: "int", nullable: false),
                    ProcurementItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcurementItemGroupsMembership", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcurementItemGroupsMembership_ProcurementItemGroups_ProcurementItemGroupId",
                        column: x => x.ProcurementItemGroupId,
                        principalSchema: "pcm",
                        principalTable: "ProcurementItemGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProcurementItemGroupsMembership_ProcurementItem_ProcurementItemId",
                        column: x => x.ProcurementItemId,
                        principalSchema: "pcm",
                        principalTable: "ProcurementItem",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementItemGroupsMembership_ProcurementItemGroupId",
                schema: "pcm",
                table: "ProcurementItemGroupsMembership",
                column: "ProcurementItemGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementItemGroupsMembership_ProcurementItemId",
                schema: "pcm",
                table: "ProcurementItemGroupsMembership",
                column: "ProcurementItemId");
        }
    }
}
