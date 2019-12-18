using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationPortal.Web.Migrations
{
    public partial class AddedUserIdToTestCompletionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AnswerHistoryData");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "TestCompletions",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TestCompletions");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AnswerHistoryData",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
