using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationPortal.Web.Migrations
{
    public partial class RemovedNumberOfPointsFromAnswerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfPoints",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "NumberOfPoints",
                table: "AnswerHistories");

            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "Questions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageContentType",
                table: "Questions");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPoints",
                table: "Answers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPoints",
                table: "AnswerHistories",
                nullable: false,
                defaultValue: 0);
        }
    }
}
