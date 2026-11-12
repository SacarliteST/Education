using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Education.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteContentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PracticalModuleSessions_PracticalTaskAssignments_assignment~",
                table: "PracticalModuleSessions");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "TheoreticalMaterials",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "TheoreticalMaterialLinks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "TheoreticalMaterialFiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "Questions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "PracticalMaterials",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "Modules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "Courses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "Cases",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PracticalModuleSessions_PracticalTaskAssignments_assignment~",
                table: "PracticalModuleSessions",
                column: "assignment_id",
                principalTable: "PracticalTaskAssignments",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PracticalModuleSessions_PracticalTaskAssignments_assignment~",
                table: "PracticalModuleSessions");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "TheoreticalMaterials");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "TheoreticalMaterialLinks");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "TheoreticalMaterialFiles");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "PracticalMaterials");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "Cases");

            migrationBuilder.AddForeignKey(
                name: "FK_PracticalModuleSessions_PracticalTaskAssignments_assignment~",
                table: "PracticalModuleSessions",
                column: "assignment_id",
                principalTable: "PracticalTaskAssignments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
