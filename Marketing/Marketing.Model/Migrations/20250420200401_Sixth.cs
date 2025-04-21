using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketing.Model.Migrations
{
    /// <inheritdoc />
    public partial class Sixth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomersGroupMemberShip_CustomerGroups_GroupId",
                schema: "mrk",
                table: "CustomersGroupMemberShip");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomersGroupMemberShip_Customer_CustomerId",
                schema: "mrk",
                table: "CustomersGroupMemberShip");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomersGroupMemberShip",
                schema: "mrk",
                table: "CustomersGroupMemberShip");

            migrationBuilder.RenameTable(
                name: "CustomersGroupMemberShip",
                schema: "mrk",
                newName: "CustomersGroupMembership",
                newSchema: "mrk");

            migrationBuilder.RenameIndex(
                name: "IX_CustomersGroupMemberShip_GroupId",
                schema: "mrk",
                table: "CustomersGroupMembership",
                newName: "IX_CustomersGroupMembership_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomersGroupMemberShip_CustomerId",
                schema: "mrk",
                table: "CustomersGroupMembership",
                newName: "IX_CustomersGroupMembership_CustomerId");

            migrationBuilder.AddColumn<int>(
                name: "CustomerNumber",
                schema: "mrk",
                table: "Customer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomersGroupMembership",
                schema: "mrk",
                table: "CustomersGroupMembership",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersGroupMembership_CustomerGroups_GroupId",
                schema: "mrk",
                table: "CustomersGroupMembership",
                column: "GroupId",
                principalSchema: "mrk",
                principalTable: "CustomerGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersGroupMembership_Customer_CustomerId",
                schema: "mrk",
                table: "CustomersGroupMembership",
                column: "CustomerId",
                principalSchema: "mrk",
                principalTable: "Customer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomersGroupMembership_CustomerGroups_GroupId",
                schema: "mrk",
                table: "CustomersGroupMembership");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomersGroupMembership_Customer_CustomerId",
                schema: "mrk",
                table: "CustomersGroupMembership");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomersGroupMembership",
                schema: "mrk",
                table: "CustomersGroupMembership");

            migrationBuilder.DropColumn(
                name: "CustomerNumber",
                schema: "mrk",
                table: "Customer");

            migrationBuilder.RenameTable(
                name: "CustomersGroupMembership",
                schema: "mrk",
                newName: "CustomersGroupMemberShip",
                newSchema: "mrk");

            migrationBuilder.RenameIndex(
                name: "IX_CustomersGroupMembership_GroupId",
                schema: "mrk",
                table: "CustomersGroupMemberShip",
                newName: "IX_CustomersGroupMemberShip_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomersGroupMembership_CustomerId",
                schema: "mrk",
                table: "CustomersGroupMemberShip",
                newName: "IX_CustomersGroupMemberShip_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomersGroupMemberShip",
                schema: "mrk",
                table: "CustomersGroupMemberShip",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersGroupMemberShip_CustomerGroups_GroupId",
                schema: "mrk",
                table: "CustomersGroupMemberShip",
                column: "GroupId",
                principalSchema: "mrk",
                principalTable: "CustomerGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersGroupMemberShip_Customer_CustomerId",
                schema: "mrk",
                table: "CustomersGroupMemberShip",
                column: "CustomerId",
                principalSchema: "mrk",
                principalTable: "Customer",
                principalColumn: "Id");
        }
    }
}
