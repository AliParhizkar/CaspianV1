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

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "cmn",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                computedColumnSql: "[FName] + ' ' + [LName]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
