using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketing.Model.Migrations
{
    /// <inheritdoc />
    public partial class Version3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                schema: "Hr",
                table: "Secretariat",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Secretariat_ParentId",
                schema: "Hr",
                table: "Secretariat",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Secretariat_Secretariat_ParentId",
                schema: "Hr",
                table: "Secretariat",
                column: "ParentId",
                principalSchema: "Hr",
                principalTable: "Secretariat",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Secretariat_Secretariat_ParentId",
                schema: "Hr",
                table: "Secretariat");

            migrationBuilder.DropIndex(
                name: "IX_Secretariat_ParentId",
                schema: "Hr",
                table: "Secretariat");

            migrationBuilder.DropColumn(
                name: "ParentId",
                schema: "Hr",
                table: "Secretariat");
        }
    }
}
