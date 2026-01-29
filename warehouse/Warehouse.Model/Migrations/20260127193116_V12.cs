using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehouse.Model.Migrations
{
    /// <inheritdoc />
    public partial class V12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasurementUnits",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MeasurementDimension = table.Column<byte>(type: "tinyint", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StandardChangeUnits",
                schema: "wh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MainUnitId = table.Column<int>(type: "int", nullable: false),
                    OtherUnitId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardChangeUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StandardChangeUnits_MeasurementUnits_MainUnitId",
                        column: x => x.MainUnitId,
                        principalSchema: "wh",
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StandardChangeUnits_MeasurementUnits_OtherUnitId",
                        column: x => x.OtherUnitId,
                        principalSchema: "wh",
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StandardChangeUnits_MainUnitId",
                schema: "wh",
                table: "StandardChangeUnits",
                column: "MainUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_StandardChangeUnits_OtherUnitId",
                schema: "wh",
                table: "StandardChangeUnits",
                column: "OtherUnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StandardChangeUnits",
                schema: "wh");

            migrationBuilder.DropTable(
                name: "MeasurementUnits",
                schema: "wh");
        }
    }
}
