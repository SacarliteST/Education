using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Education.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskSelectionRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "practical_task_id",
                table: "PracticalModuleSessions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "assignment_id",
                table: "PracticalModuleSessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PracticalTaskAssignments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    practical_material_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    class_window_id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_task_ref = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticalTaskAssignments", x => x.id);
                    table.ForeignKey(
                        name: "FK_PracticalTaskAssignments_ClassWindows_class_window_id",
                        column: x => x.class_window_id,
                        principalTable: "ClassWindows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PracticalTaskAssignments_PracticalMaterials_practical_mater~",
                        column: x => x.practical_material_id,
                        principalTable: "PracticalMaterials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskSelectionRules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    practical_material_id = table.Column<Guid>(type: "uuid", nullable: false),
                    practical_module_id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_count = table.Column<int>(type: "integer", nullable: false),
                    difficulty_levels = table.Column<List<short>>(type: "smallint[]", nullable: false),
                    topic_refs = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSelectionRules", x => x.id);
                    table.ForeignKey(
                        name: "FK_TaskSelectionRules_PracticalMaterials_practical_material_id",
                        column: x => x.practical_material_id,
                        principalTable: "PracticalMaterials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskSelectionRules_PracticalModules_practical_module_id",
                        column: x => x.practical_module_id,
                        principalTable: "PracticalModules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PracticalModuleSessions_assignment_id",
                table: "PracticalModuleSessions",
                column: "assignment_id");

            migrationBuilder.CreateIndex(
                name: "IX_PracticalTaskAssignments_class_window_id",
                table: "PracticalTaskAssignments",
                column: "class_window_id");

            migrationBuilder.CreateIndex(
                name: "IX_PracticalTaskAssignments_practical_material_id_class_window~",
                table: "PracticalTaskAssignments",
                columns: new[] { "practical_material_id", "class_window_id" });

            migrationBuilder.CreateIndex(
                name: "IX_PracticalTaskAssignments_practical_material_id_student_id_c~",
                table: "PracticalTaskAssignments",
                columns: new[] { "practical_material_id", "student_id", "class_window_id", "external_task_ref" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskSelectionRules_practical_material_id",
                table: "TaskSelectionRules",
                column: "practical_material_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskSelectionRules_practical_module_id",
                table: "TaskSelectionRules",
                column: "practical_module_id");

            migrationBuilder.AddForeignKey(
                name: "FK_PracticalModuleSessions_PracticalTaskAssignments_assignment~",
                table: "PracticalModuleSessions",
                column: "assignment_id",
                principalTable: "PracticalTaskAssignments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PracticalModuleSessions_PracticalTaskAssignments_assignment~",
                table: "PracticalModuleSessions");

            migrationBuilder.DropTable(
                name: "PracticalTaskAssignments");

            migrationBuilder.DropTable(
                name: "TaskSelectionRules");

            migrationBuilder.DropIndex(
                name: "IX_PracticalModuleSessions_assignment_id",
                table: "PracticalModuleSessions");

            migrationBuilder.DropColumn(
                name: "assignment_id",
                table: "PracticalModuleSessions");

            migrationBuilder.AlterColumn<Guid>(
                name: "practical_task_id",
                table: "PracticalModuleSessions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
