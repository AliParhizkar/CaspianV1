using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Model.Migrations
{
    /// <inheritdoc />
    public partial class AccountCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountCoding",
                schema: "Demo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    ParentCodeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCoding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountCoding_AccountCoding_ParentCodeId",
                        column: x => x.ParentCodeId,
                        principalSchema: "Demo",
                        principalTable: "AccountCoding",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountCoding_ParentCodeId",
                schema: "Demo",
                table: "AccountCoding",
                column: "ParentCodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountCoding",
                schema: "Demo");
        }
    }
}
