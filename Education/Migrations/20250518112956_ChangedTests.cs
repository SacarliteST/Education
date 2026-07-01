using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Education.Migrations
{
    /// <inheritdoc />
    public partial class ChangedTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "turned_in_date",
                table: "TestResults",
                newName: "started_at");

            migrationBuilder.AlterColumn<double>(
                name: "score",
                table: "TestResults",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "max_score",
                table: "TestResults",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<bool>(
                name: "is_completed",
                table: "TestResults",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "turned_at",
                table: "TestResults",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tries_count",
                table: "PracticalMaterials",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_completed",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "turned_at",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "tries_count",
                table: "PracticalMaterials");

            migrationBuilder.RenameColumn(
                name: "started_at",
                table: "TestResults",
                newName: "turned_in_date");

            migrationBuilder.AlterColumn<double>(
                name: "score",
                table: "TestResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "max_score",
                table: "TestResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);
        }
    }
}
