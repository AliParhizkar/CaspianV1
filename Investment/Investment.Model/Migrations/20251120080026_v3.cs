using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investment.Model.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganUnits",
                schema: "ivm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Descript = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    ParentOrganUnitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganUnits_OrganUnits_ParentOrganUnitId",
                        column: x => x.ParentOrganUnitId,
                        principalSchema: "ivm",
                        principalTable: "OrganUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganUnits_ParentOrganUnitId",
                schema: "ivm",
                table: "OrganUnits",
                column: "ParentOrganUnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganUnits",
                schema: "ivm");
        }
    }
}
