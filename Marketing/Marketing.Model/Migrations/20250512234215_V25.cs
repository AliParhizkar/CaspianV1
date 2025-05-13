using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketing.Model.Migrations
{
    /// <inheritdoc />
    public partial class V25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "OrderTime",
                schema: "mrk",
                table: "Orders",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.CreateTable(
                name: "CustomersAccountings",
                schema: "mrk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Payment = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomersAccountings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomersAccountings_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "mrk",
                        principalTable: "Customer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomersAccountings_CustomerId",
                schema: "mrk",
                table: "CustomersAccountings",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomersAccountings",
                schema: "mrk");

            migrationBuilder.DropColumn(
                name: "OrderTime",
                schema: "mrk",
                table: "Orders");
        }
    }
}
