using Microsoft.EntityFrameworkCore.Migrations;

namespace Rocky.Migrations
{
    public partial class AddWellTableChange1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SoilPropertiesId",
                table: "Well",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Well_SoilPropertiesId",
                table: "Well",
                column: "SoilPropertiesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Well_SoilProperties_SoilPropertiesId",
                table: "Well",
                column: "SoilPropertiesId",
                principalTable: "SoilProperties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Well_SoilProperties_SoilPropertiesId",
                table: "Well");

            migrationBuilder.DropIndex(
                name: "IX_Well_SoilPropertiesId",
                table: "Well");

            migrationBuilder.DropColumn(
                name: "SoilPropertiesId",
                table: "Well");
        }
    }
}
