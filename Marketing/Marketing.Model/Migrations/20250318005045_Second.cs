using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketing.Model.Migrations
{
    /// <inheritdoc />
    public partial class Second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerGroups_Customer_CustomerId",
                schema: "mrk",
                table: "CustomerGroups");

            migrationBuilder.DropIndex(
                name: "IX_CustomerGroups_CustomerId",
                schema: "mrk",
                table: "CustomerGroups");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                schema: "mrk",
                table: "CustomerGroups");

            migrationBuilder.AddColumn<string>(
                name: "Address1",
                schema: "mrk",
                table: "Customer",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                schema: "mrk",
                table: "Customer",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                schema: "mrk",
                table: "Customer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomersGroupMemberShip",
                schema: "mrk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomersGroupMemberShip", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomersGroupMemberShip_CustomerGroups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "mrk",
                        principalTable: "CustomerGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomersGroupMemberShip_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "mrk",
                        principalTable: "Customer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MerchantsConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerHasManyAddresses = table.Column<bool>(type: "bit", nullable: false),
                    CustomerIsMemberOfGroups = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantsConfig", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_GroupId",
                schema: "mrk",
                table: "Customer",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomersGroupMemberShip_CustomerId",
                schema: "mrk",
                table: "CustomersGroupMemberShip",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomersGroupMemberShip_GroupId",
                schema: "mrk",
                table: "CustomersGroupMemberShip",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_CustomerGroups_GroupId",
                schema: "mrk",
                table: "Customer",
                column: "GroupId",
                principalSchema: "mrk",
                principalTable: "CustomerGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_CustomerGroups_GroupId",
                schema: "mrk",
                table: "Customer");

            migrationBuilder.DropTable(
                name: "CustomersGroupMemberShip",
                schema: "mrk");

            migrationBuilder.DropTable(
                name: "MerchantsConfig");

            migrationBuilder.DropIndex(
                name: "IX_Customer_GroupId",
                schema: "mrk",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Address1",
                schema: "mrk",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Address2",
                schema: "mrk",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "GroupId",
                schema: "mrk",
                table: "Customer");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                schema: "mrk",
                table: "CustomerGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerGroups_CustomerId",
                schema: "mrk",
                table: "CustomerGroups",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerGroups_Customer_CustomerId",
                schema: "mrk",
                table: "CustomerGroups",
                column: "CustomerId",
                principalSchema: "mrk",
                principalTable: "Customer",
                principalColumn: "Id");
        }
    }
}
