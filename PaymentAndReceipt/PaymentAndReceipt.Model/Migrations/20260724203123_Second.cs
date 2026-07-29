using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentAndReceipt.Model.Migrations
{
    /// <inheritdoc />
    public partial class Second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "ActiveType",
                schema: "par",
                table: "Currencies",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveType",
                schema: "par",
                table: "Currencies");
        }
    }
}
