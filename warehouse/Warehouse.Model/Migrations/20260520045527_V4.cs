using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_InventoryControlAgents_GoodsPropertiesId",
                schema: "wh",
                table: "InventoryControlAgents",
                column: "GoodsPropertiesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryControlAgents",
                schema: "wh");
        }
    }
}
