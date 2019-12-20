using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationPortal.Web.Migrations
{
    public partial class AddedMaxNumbetOfAttemptsToTestsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxNumberOfAttempts",
                table: "Tests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Questions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxNumberOfAttempts",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Questions");
        }
    }
}
