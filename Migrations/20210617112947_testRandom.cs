using Microsoft.EntityFrameworkCore.Migrations;

namespace KRU.Migrations
{
    public partial class testRandom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RandomQuestions",
                table: "Tests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RandomQuestions",
                table: "Tests");
        }
    }
}
