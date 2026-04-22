using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Procurement.Model.Migrations
{
    /// <inheritdoc />
    public partial class V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupplierGroups",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ParentGroupId = table.Column<int>(type: "int", nullable: true)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierGroupMemberships",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "SupplierGroups",
                schema: "pcm");
        }
    }
}
