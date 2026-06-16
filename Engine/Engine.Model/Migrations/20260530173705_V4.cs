using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Engine.Model.Migrations
{
    /// <inheritdoc />
    public partial class V4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubSystemKind",
                schema: "cmn",
                table: "WorkflowGroups",
                newName: "SubsystemKind");

            migrationBuilder.RenameColumn(
                name: "SubSystemKind",
                schema: "cmn",
                table: "Menus",
                newName: "SubsystemKind");

            migrationBuilder.RenameColumn(
                name: "ShowonMenu",
                schema: "cmn",
                table: "Menus",
                newName: "ShowOnMenu");

            migrationBuilder.RenameColumn(
                name: "SubSystemKind",
                schema: "cmn",
                table: "MenuCategories",
                newName: "SubsystemKind");

            migrationBuilder.RenameColumn(
                name: "SubSystem",
                schema: "cmn",
                table: "EntityTypes",
                newName: "Subsystem");

            migrationBuilder.AlterColumn<string>(
                name: "PageUrl",
                schema: "cmn",
                table: "UsersLogins",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                schema: "cmn",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "cmn",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                computedColumnSql: "[FName] + ' ' + [LName]");

            migrationBuilder.CreateTable(
                name: "UserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaim", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserClaim");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "cmn",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "SubsystemKind",
                schema: "cmn",
                table: "WorkflowGroups",
                newName: "SubSystemKind");

            migrationBuilder.RenameColumn(
                name: "SubsystemKind",
                schema: "cmn",
                table: "Menus",
                newName: "SubSystemKind");

            migrationBuilder.RenameColumn(
                name: "ShowOnMenu",
                schema: "cmn",
                table: "Menus",
                newName: "ShowonMenu");

            migrationBuilder.RenameColumn(
                name: "SubsystemKind",
                schema: "cmn",
                table: "MenuCategories",
                newName: "SubSystemKind");

            migrationBuilder.RenameColumn(
                name: "Subsystem",
                schema: "cmn",
                table: "EntityTypes",
                newName: "SubSystem");

            migrationBuilder.AlterColumn<string>(
                name: "PageUrl",
                schema: "cmn",
                table: "UsersLogins",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                schema: "cmn",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
