using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Education.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskSelectionDifficultyDistribution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<short>>(
                name: "difficulty_distribution",
                table: "TaskSelectionRules",
                type: "smallint[]",
                nullable: false,
                defaultValueSql: "ARRAY[]::smallint[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "difficulty_distribution",
                table: "TaskSelectionRules");
        }
    }
}
