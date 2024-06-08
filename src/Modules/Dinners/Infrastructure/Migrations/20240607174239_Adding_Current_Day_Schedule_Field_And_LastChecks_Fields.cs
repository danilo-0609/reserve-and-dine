using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dinners.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Adding_Current_Day_Schedule_Field_And_LastChecks_Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Speciality",
                schema: "dinners",
                table: "Specialties",
                newName: "Specialty");

            migrationBuilder.AddColumn<int>(
                name: "CurrentDaySchedule",
                schema: "dinners",
                table: "Restaurants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCheckedCurrentDaySchedule",
                schema: "dinners",
                table: "Restaurants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCheckedScheduleStatus",
                schema: "dinners",
                table: "Restaurants",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentDaySchedule",
                schema: "dinners",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "LastCheckedCurrentDaySchedule",
                schema: "dinners",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "LastCheckedScheduleStatus",
                schema: "dinners",
                table: "Restaurants");

            migrationBuilder.RenameColumn(
                name: "Specialty",
                schema: "dinners",
                table: "Specialties",
                newName: "Speciality");
        }
    }
}
