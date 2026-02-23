using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goods_CodingPatterns_CodingPatternId",
                schema: "wh",
                table: "Goods");

            migrationBuilder.DropTable(
                name: "CodingPatterns",
                schema: "wh");

            migrationBuilder.RenameColumn(
                name: "CodingPatternId",
                schema: "wh",
                table: "Goods",
                newName: "BarcodePatternId");

            migrationBuilder.RenameIndex(
                name: "IX_Goods_CodingPatternId",
                schema: "wh",
                table: "Goods",
                newName: "IX_Goods_BarcodePatternId");

            migrationBuilder.AddColumn<byte>(
                name: "GoodsType",
                schema: "wh",
                table: "Goods",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "BarcodePatterns",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarcodePatterns", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Goods_BarcodePatterns_BarcodePatternId",
                schema: "wh",
                table: "Goods",
                column: "BarcodePatternId",
                principalSchema: "wh",
                principalTable: "BarcodePatterns",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goods_BarcodePatterns_BarcodePatternId",
                schema: "wh",
                table: "Goods");

            migrationBuilder.DropTable(
                name: "BarcodePatterns",
                schema: "wh");

            migrationBuilder.DropColumn(
                name: "GoodsType",
                schema: "wh",
                table: "Goods");

            migrationBuilder.RenameColumn(
                name: "BarcodePatternId",
                schema: "wh",
                table: "Goods",
                newName: "CodingPatternId");

            migrationBuilder.RenameIndex(
                name: "IX_Goods_BarcodePatternId",
                schema: "wh",
                table: "Goods",
                newName: "IX_Goods_CodingPatternId");

            migrationBuilder.CreateTable(
                name: "CodingPatterns",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodingPatterns", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Goods_CodingPatterns_CodingPatternId",
                schema: "wh",
                table: "Goods",
                column: "CodingPatternId",
                principalSchema: "wh",
                principalTable: "CodingPatterns",
                principalColumn: "Id");
        }
    }
}
