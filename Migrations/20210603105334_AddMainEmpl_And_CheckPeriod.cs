using Microsoft.EntityFrameworkCore.Migrations;

namespace KRU.Migrations
{
    public partial class AddMainEmpl_And_CheckPeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MasulEmplId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckPeriod",
                table: "FinanceReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_MasulEmplId",
                table: "Tasks",
                column: "MasulEmplId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Employees_MasulEmplId",
                table: "Tasks",
                column: "MasulEmplId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Employees_MasulEmplId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_MasulEmplId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "MasulEmplId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CheckPeriod",
                table: "FinanceReports");
        }
    }
}
