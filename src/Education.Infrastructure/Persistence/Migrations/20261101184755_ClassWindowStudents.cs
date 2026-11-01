using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Education.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ClassWindowStudents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClassWindows_course_id_group_name",
                table: "ClassWindows");

            migrationBuilder.AlterColumn<string>(
                name: "group_name",
                table: "ClassWindows",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "ClassWindowStudents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    class_window_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassWindowStudents", x => x.id);
                    table.ForeignKey(
                        name: "FK_ClassWindowStudents_ClassWindows_class_window_id",
                        column: x => x.class_window_id,
                        principalTable: "ClassWindows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassWindows_course_id_group_name",
                table: "ClassWindows",
                columns: new[] { "course_id", "group_name" },
                unique: true,
                filter: "closed_at IS NULL AND group_name IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ClassWindowStudents_class_window_id_user_id",
                table: "ClassWindowStudents",
                columns: new[] { "class_window_id", "user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassWindowStudents");

            migrationBuilder.DropIndex(
                name: "IX_ClassWindows_course_id_group_name",
                table: "ClassWindows");

            migrationBuilder.AlterColumn<string>(
                name: "group_name",
                table: "ClassWindows",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassWindows_course_id_group_name",
                table: "ClassWindows",
                columns: new[] { "course_id", "group_name" },
                unique: true,
                filter: "closed_at IS NULL");
        }
    }
}
