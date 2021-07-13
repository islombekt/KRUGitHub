using Microsoft.EntityFrameworkCore.Migrations;

namespace KRU.Migrations
{
    public partial class Task_Report : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentTR",
                table: "Task_Empls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Task_Reports",
                columns: table => new
                {
                    Task_RepId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: true),
                    RepId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task_Reports", x => x.Task_RepId);
                    table.ForeignKey(
                        name: "FK_Task_Reports_Reports_RepId",
                        column: x => x.RepId,
                        principalTable: "Reports",
                        principalColumn: "ReportId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Task_Reports_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Task_Reports_RepId",
                table: "Task_Reports",
                column: "RepId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_Reports_TaskId",
                table: "Task_Reports",
                column: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Task_Reports");

            migrationBuilder.DropColumn(
                name: "CommentTR",
                table: "Task_Empls");
        }
    }
}
