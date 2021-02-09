using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KRU.Data.Migrations
{
    public partial class File_And_Register : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HaveSeen",
                table: "Reports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HaveSeen",
                table: "Plans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EnteredToWork",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FiredFromWork",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HaveSeen",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "HaveSeen",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EnteredToWork",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FiredFromWork",
                table: "AspNetUsers");
        }
    }
}
