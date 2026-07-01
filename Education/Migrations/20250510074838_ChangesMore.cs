using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Education.Migrations
{
    /// <inheritdoc />
    public partial class ChangesMore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_PracticalMaterialBindQuestions_PracticalMaterialBin~",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "grade",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "percent",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "question_list_bind_question_id",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "PracticalMaterialBindQuestionId",
                table: "Answers",
                newName: "practical_material_bind_question_id");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_PracticalMaterialBindQuestionId",
                table: "Answers",
                newName: "IX_Answers_practical_material_bind_question_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_PracticalMaterialBindQuestions_practical_material_b~",
                table: "Answers",
                column: "practical_material_bind_question_id",
                principalTable: "PracticalMaterialBindQuestions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_PracticalMaterialBindQuestions_practical_material_b~",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "practical_material_bind_question_id",
                table: "Answers",
                newName: "PracticalMaterialBindQuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_practical_material_bind_question_id",
                table: "Answers",
                newName: "IX_Answers_PracticalMaterialBindQuestionId");

            migrationBuilder.AddColumn<int>(
                name: "grade",
                table: "TestResults",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "percent",
                table: "TestResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<long>(
                name: "question_list_bind_question_id",
                table: "Answers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_PracticalMaterialBindQuestions_PracticalMaterialBin~",
                table: "Answers",
                column: "PracticalMaterialBindQuestionId",
                principalTable: "PracticalMaterialBindQuestions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
