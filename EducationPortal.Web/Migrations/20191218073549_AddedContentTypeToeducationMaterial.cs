using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationPortal.Web.Migrations
{
    public partial class AddedContentTypeToeducationMaterial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "EducationMaterials",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "EducationMaterials");
        }
    }
}
