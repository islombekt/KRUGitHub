using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KRU.Data.Migrations
{
    public partial class File : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentType",
                table: "Task_Types");

            migrationBuilder.DropColumn(
                name: "Finished",
                table: "Task_Types");

            migrationBuilder.DropColumn(
                name: "TaskTypeEnd",
                table: "Task_Types");

            migrationBuilder.DropColumn(
                name: "TaskTypeStarted",
                table: "Task_Types");

            migrationBuilder.RenameColumn(
                name: "TaskState",
                table: "Tasks",
                newName: "File");

            migrationBuilder.AddColumn<bool>(
                name: "Finished",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FileHistory",
                columns: table => new
                {
                    FileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileFinished = table.Column<bool>(type: "bit", nullable: false),
                    TaskTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileHistory", x => x.FileId);
                    table.ForeignKey(
                        name: "FK_FileHistory_Task_Types_TaskTypeId",
                        column: x => x.TaskTypeId,
                        principalTable: "Task_Types",
                        principalColumn: "TaskTypeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Task_Files",
                columns: table => new
                {
                    Task_FileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: true),
                    FileId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task_Files", x => x.Task_FileId);
                    table.ForeignKey(
                        name: "FK_Task_Files_FileHistory_FileId",
                        column: x => x.FileId,
                        principalTable: "FileHistory",
                        principalColumn: "FileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Task_Files_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileHistory_TaskTypeId",
                table: "FileHistory",
                column: "TaskTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_Files_FileId",
                table: "Task_Files",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_Files_TaskId",
                table: "Task_Files",
                column: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Task_Files");

            migrationBuilder.DropTable(
                name: "FileHistory");

            migrationBuilder.DropColumn(
                name: "Finished",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "File",
                table: "Tasks",
                newName: "TaskState");

            migrationBuilder.AddColumn<string>(
                name: "CommentType",
                table: "Task_Types",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Finished",
                table: "Task_Types",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TaskTypeEnd",
                table: "Task_Types",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TaskTypeStarted",
                table: "Task_Types",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
