using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationPortal.Web.Migrations
{
    public partial class AddedModulesDbSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationMaterials_Module_ModuleId",
                table: "EducationMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_Module_Courses_CourseId",
                table: "Module");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Module_ModuleId",
                table: "Tests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Module",
                table: "Module");

            migrationBuilder.RenameTable(
                name: "Module",
                newName: "Modules");

            migrationBuilder.RenameIndex(
                name: "IX_Module_CourseId",
                table: "Modules",
                newName: "IX_Modules_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Modules",
                table: "Modules",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationMaterials_Modules_ModuleId",
                table: "EducationMaterials",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Courses_CourseId",
                table: "Modules",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Modules_ModuleId",
                table: "Tests",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationMaterials_Modules_ModuleId",
                table: "EducationMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Courses_CourseId",
                table: "Modules");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Modules_ModuleId",
                table: "Tests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Modules",
                table: "Modules");

            migrationBuilder.RenameTable(
                name: "Modules",
                newName: "Module");

            migrationBuilder.RenameIndex(
                name: "IX_Modules_CourseId",
                table: "Module",
                newName: "IX_Module_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Module",
                table: "Module",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationMaterials_Module_ModuleId",
                table: "EducationMaterials",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Module_Courses_CourseId",
                table: "Module",
                column: "CourseId",
                principalTable: "Courses",
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
    }
}
