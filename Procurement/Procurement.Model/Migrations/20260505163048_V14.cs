using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Procurement.Model.Migrations
{
    /// <inheritdoc />
    public partial class V14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sellers",
                schema: "pcm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonType = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EconomicCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Citizenship = table.Column<byte>(type: "tinyint", nullable: false),
                    IdCard = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PostCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AreaCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tell = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tell2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sellers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sellers_Locations_CityId",
                        column: x => x.CityId,
                        principalSchema: "wh",
                        principalTable: "Locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sellers_Locations_ProvinceId",
                        column: x => x.ProvinceId,
                        principalSchema: "wh",
                        principalTable: "Locations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_CityId",
                schema: "pcm",
                table: "Sellers",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_ProvinceId",
                schema: "pcm",
                table: "Sellers",
                column: "ProvinceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sellers",
                schema: "pcm");
        }
    }
}
