using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Procurement.Model.Migrations
{
    /// <inheritdoc />
    public partial class V4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "ProcurementItemGroupsMembership",
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
                name: "IX_ProcurementItemGroups_ParentGroupId",
                schema: "pcm",
                table: "ProcurementItemGroups",
                column: "ParentGroupId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcurementItemGroupsMembership",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "ProcurementItemGroups",
                schema: "pcm");
        }
    }
}
