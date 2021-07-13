using Microsoft.EntityFrameworkCore.Migrations;

namespace KRU.Migrations
{
    public partial class fileTaskEndcancel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrlEnd",
                table: "Tasks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileUrlEnd",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
