using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Education.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGrading : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "percent_for_five",
                table: "PracticalMaterials",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "percent_for_four",
                table: "PracticalMaterials",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "percent_for_three",
                table: "PracticalMaterials",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "grade",
                table: "CaseFiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "percent_for_five",
                table: "PracticalMaterials");

            migrationBuilder.DropColumn(
                name: "percent_for_four",
                table: "PracticalMaterials");

            migrationBuilder.DropColumn(
                name: "percent_for_three",
                table: "PracticalMaterials");

            migrationBuilder.DropColumn(
                name: "grade",
                table: "CaseFiles");
        }
    }
}
