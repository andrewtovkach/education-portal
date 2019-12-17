using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationPortal.Web.Migrations
{
    public partial class AnswerHistoryTableChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextInput",
                table: "AnswerHistoryData");

            migrationBuilder.AddColumn<string>(
                name: "TextInput",
                table: "AnswerHistories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextInput",
                table: "AnswerHistories");

            migrationBuilder.AddColumn<string>(
                name: "TextInput",
                table: "AnswerHistoryData",
                nullable: true);
        }
    }
}
