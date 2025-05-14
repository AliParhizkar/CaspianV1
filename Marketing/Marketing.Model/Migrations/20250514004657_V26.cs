using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketing.Model.Migrations
{
    /// <inheritdoc />
    public partial class V26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSpecialCustomer",
                schema: "mrk",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSpecialCustomer",
                schema: "mrk",
                table: "Orders");
        }
    }
}
