using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationPortal.Web.Migrations
{
    public partial class AddedModulesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationMaterials_Courses_CourseId",
                table: "EducationMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Courses_CourseId",
                table: "Tests");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Tests",
                newName: "ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_CourseId",
                table: "Tests",
                newName: "IX_Tests_ModuleId");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "EducationMaterials",
                newName: "ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_EducationMaterials_CourseId",
                table: "EducationMaterials",
                newName: "IX_EducationMaterials_ModuleId");

            migrationBuilder.CreateTable(
                name: "Module",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    CourseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Module", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Module_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Module_CourseId",
                table: "Module",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationMaterials_Module_ModuleId",
                table: "EducationMaterials",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Module_ModuleId",
                table: "Tests",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationMaterials_Module_ModuleId",
                table: "EducationMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Module_ModuleId",
                table: "Tests");

            migrationBuilder.DropTable(
                name: "Module");

            migrationBuilder.RenameColumn(
                name: "ModuleId",
                table: "Tests",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_ModuleId",
                table: "Tests",
                newName: "IX_Tests_CourseId");

            migrationBuilder.RenameColumn(
                name: "ModuleId",
                table: "EducationMaterials",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_EducationMaterials_ModuleId",
                table: "EducationMaterials",
                newName: "IX_EducationMaterials_CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationMaterials_Courses_CourseId",
                table: "EducationMaterials",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Courses_CourseId",
                table: "Tests",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
