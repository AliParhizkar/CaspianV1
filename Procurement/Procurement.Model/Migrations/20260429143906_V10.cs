using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Procurement.Model.Migrations
{
    /// <inheritdoc />
    public partial class V10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Policies",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PolicyKind = table.Column<byte>(type: "tinyint", nullable: false),
                    EffectingLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    CalculatingMethod = table.Column<byte>(type: "tinyint", nullable: false),
                    ParticipatoryApproach = table.Column<byte>(type: "tinyint", nullable: false),
                    Definiteness = table.Column<byte>(type: "tinyint", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyParameters",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Property = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyParameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyParametersMembership",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyId = table.Column<int>(type: "int", nullable: false),
                    ParameterId = table.Column<int>(type: "int", nullable: false)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PolicyParametersMembership",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "Policies",
                schema: "pcm");

            migrationBuilder.DropTable(
                name: "PolicyParameters",
                schema: "pcm");
        }
    }
}
