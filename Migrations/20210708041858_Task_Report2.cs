using Microsoft.EntityFrameworkCore.Migrations;

namespace KRU.Migrations
{
    public partial class Task_Report2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentTR",
                table: "Task_Empls");

            migrationBuilder.AddColumn<string>(
                name: "CommentTR",
                table: "Task_Reports",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentTR",
                table: "Task_Reports");

            migrationBuilder.AddColumn<int>(
                name: "CommentTR",
                table: "Task_Empls",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
