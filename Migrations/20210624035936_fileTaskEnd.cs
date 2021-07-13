using Microsoft.EntityFrameworkCore.Migrations;

namespace KRU.Migrations
{
    public partial class fileTaskEnd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileUrlEnd",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrlEnd",
                table: "Tasks");
        }
    }
}
