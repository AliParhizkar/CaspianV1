using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class v32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentPatterns",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DocumentType = table.Column<byte>(type: "tinyint", nullable: false),
                    SaleType = table.Column<byte>(type: "tinyint", nullable: true),
                    InOutType = table.Column<byte>(type: "tinyint", nullable: false),
                    InventoryImpactType = table.Column<byte>(type: "tinyint", nullable: false),
                    RowsCount = table.Column<int>(type: "int", nullable: false),
                    FromStockRoomFieldName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HasToStockRoom = table.Column<bool>(type: "bit", nullable: false),
                    ToStockRoomFieldName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HasReceiverIssuer = table.Column<bool>(type: "bit", nullable: false),
                    ReceiverIssuerFieldName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DocumentRelationshipType = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentPatterns", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentPatterns",
                schema: "wh");
        }
    }
}
