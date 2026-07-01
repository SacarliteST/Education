using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Education.Migrations
{
    /// <inheritdoc />
    public partial class Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Users_user_id",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "is_correct",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Answers",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_user_id",
                table: "Answers",
                newName: "IX_Answers_UserId");

            migrationBuilder.AddColumn<bool>(
                name: "is_public",
                table: "PracticalMaterials",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_accepted",
                table: "CaseFiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "Answers",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "test_result_id",
                table: "Answers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "CaseFileComments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cfc_text = table.Column<string>(type: "text", nullable: false),
                    is_generated = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CaseFileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseFileComments", x => x.id);
                    table.ForeignKey(
                        name: "FK_CaseFileComments_CaseFiles_CaseFileId",
                        column: x => x.CaseFileId,
                        principalTable: "CaseFiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    turned_in_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    score = table.Column<double>(type: "double precision", nullable: false),
                    max_score = table.Column<double>(type: "double precision", nullable: false),
                    grade = table.Column<int>(type: "integer", nullable: false),
                    percent = table.Column<double>(type: "double precision", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    practical_material_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.id);
                    table.ForeignKey(
                        name: "FK_TestResults_PracticalMaterials_practical_material_id",
                        column: x => x.practical_material_id,
                        principalTable: "PracticalMaterials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestResults_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_test_result_id",
                table: "Answers",
                column: "test_result_id");

            migrationBuilder.CreateIndex(
                name: "IX_CaseFileComments_CaseFileId",
                table: "CaseFileComments",
                column: "CaseFileId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_practical_material_id",
                table: "TestResults",
                column: "practical_material_id");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_user_id",
                table: "TestResults",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_TestResults_test_result_id",
                table: "Answers",
                column: "test_result_id",
                principalTable: "TestResults",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Users_UserId",
                table: "Answers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_TestResults_test_result_id",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Users_UserId",
                table: "Answers");

            migrationBuilder.DropTable(
                name: "CaseFileComments");

            migrationBuilder.DropTable(
                name: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_Answers_test_result_id",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "is_public",
                table: "PracticalMaterials");

            migrationBuilder.DropColumn(
                name: "is_accepted",
                table: "CaseFiles");

            migrationBuilder.DropColumn(
                name: "test_result_id",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Answers",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_UserId",
                table: "Answers",
                newName: "IX_Answers_user_id");

            migrationBuilder.AlterColumn<long>(
                name: "user_id",
                table: "Answers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_correct",
                table: "Answers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Users_user_id",
                table: "Answers",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
