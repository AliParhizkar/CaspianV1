using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Model.Migrations
{
    /// <inheritdoc />
    public partial class V3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountingCodes",
                schema: "acc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NatureType = table.Column<byte>(type: "tinyint", nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ParentCodeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingCodes_AccountingCodes_ParentCodeId",
                        column: x => x.ParentCodeId,
                        principalSchema: "acc",
                        principalTable: "AccountingCodes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingCodes_ParentCodeId",
                schema: "acc",
                table: "AccountingCodes",
                column: "ParentCodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountingCodes",
                schema: "acc");
        }
    }
}
