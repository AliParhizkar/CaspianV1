using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Procurement.Model.Migrations
{
    /// <inheritdoc />
    public partial class V12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PolicyParametersMembership",
                schema: "pcm");

            migrationBuilder.CreateTable(
                name: "PolicyParameterConditions",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyId = table.Column<int>(type: "int", nullable: false),
                    ParameterId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyParameterConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyParameterConditions_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalSchema: "pcm",
                        principalTable: "Policies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PolicyParameterConditions_PolicyParameters_ParameterId",
                        column: x => x.ParameterId,
                        principalSchema: "pcm",
                        principalTable: "PolicyParameters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PolicyParameterConditions_ParameterId",
                schema: "pcm",
                table: "PolicyParameterConditions",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyParameterConditions_PolicyId",
                schema: "pcm",
                table: "PolicyParameterConditions",
                column: "PolicyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PolicyParameterConditions",
                schema: "pcm");

            migrationBuilder.CreateTable(
                name: "PolicyParametersMembership",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParameterId = table.Column<int>(type: "int", nullable: false),
                    PolicyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyParametersMembership", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyParametersMembership_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalSchema: "pcm",
                        principalTable: "Policies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PolicyParametersMembership_PolicyParameters_ParameterId",
                        column: x => x.ParameterId,
                        principalSchema: "pcm",
                        principalTable: "PolicyParameters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PolicyParametersMembership_ParameterId",
                schema: "pcm",
                table: "PolicyParametersMembership",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyParametersMembership_PolicyId",
                schema: "pcm",
                table: "PolicyParametersMembership",
                column: "PolicyId");
        }
    }
}
