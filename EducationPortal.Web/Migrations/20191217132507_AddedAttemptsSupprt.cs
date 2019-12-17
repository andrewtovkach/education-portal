using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationPortal.Web.Migrations
{
    public partial class AddedAttemptsSupprt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttemptId",
                table: "AnswerHistoryData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AnswerHistoryData_AttemptId",
                table: "AnswerHistoryData",
                column: "AttemptId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerHistoryData_Attempts_AttemptId",
                table: "AnswerHistoryData",
                column: "AttemptId",
                principalTable: "Attempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerHistoryData_Attempts_AttemptId",
                table: "AnswerHistoryData");

            migrationBuilder.DropIndex(
                name: "IX_AnswerHistoryData_AttemptId",
                table: "AnswerHistoryData");

            migrationBuilder.DropColumn(
                name: "AttemptId",
                table: "AnswerHistoryData");
        }
    }
}
